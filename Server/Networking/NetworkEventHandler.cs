using Server.Models;
using Server.Networking.NetworkEvents;
using Server.State;
using System.Collections.Immutable;
using System.Net;

namespace Server.Networking
{
    /// <summary>
    /// Parses and processes events sent by clients through the
    /// NetworkDatagramHandler.
    /// </summary>
    public class NetworkEventHandler
    {
        /// <summary>
        /// The client's connected - their IPEndPoints and integer Ids
        /// </summary>
        private ImmutableDictionary<IPEndPoint, int> _clientIds;

        /// <summary>
        /// The ServerState at which certain parsed events are forwarded
        /// </summary>
        private ServerState _state;

        // The current client id index (increments with each additional client)
        private int _clientIdIndex = 1;

        public NetworkEventHandler(
            in NetworkDatagramHandler datagramHandler,
            in ServerState state
        ) {
            _clientIds = ImmutableDictionary<IPEndPoint, int>.Empty;
            _state = state;

            // Ensure that when the NetworkDatagramHandler loses a client connection,
            // the client is also removed from the NetworkEventHandler, and
            // the relevant information is sent to each connected client.
            datagramHandler.LostConnection += (_, callback) => {
                if(_clientIds.ContainsKey(callback.EndPoint))
                {
                    callback.SendToAll(
                        new PlayerLeft
                        {
                            CallerInfo = new DataModel(
                                Secret: "Secret",
                                _clientIds[callback.EndPoint]
                            )
                        }.CreateString(), true
                    );
                    _clientIds = _clientIds.Remove(callback.EndPoint);
                }
            };

            // Connect the NetworkDatagramHandler's output with the NetworkEventHandler's input,
            // allowing the NetworkEventHandler to respond to client datagrams
            datagramHandler.MessageRecieved += (_, callback) => HandleEvent(callback);
        }

        /// <summary>
        /// Converts incoming text into an appropriate
        /// NetworkEvent.
        /// </summary>
        /// <param name="text">The string to parse</param>
        /// <returns></returns>
        
        private NetworkEvent ParseEvent(in string text)
        {
            var args = text.Split("::"); 
            
            return args[0] switch
            {
                "ClientMovement" => new ClientMovement(args[1]),
                "Pinged" => new Pinged(),
                "PlayerJoined" => new PlayerJoined(args[1]),
                "PlayerLeft" => new PlayerLeft(args[1]),
                "Planted" => new Planted(args[1]),
                _ => null
            };
        }

        /// <summary>
        /// Takes in a callback instance, performing the relevant task given.
        /// </summary>
        /// <param name="callback">The callback which communicates with the NetworkDatagramHandler</param>
        private void HandleEvent(in DatagramCallback callback)
        {
            switch (ParseEvent(callback.Data))
            {
                case PlayerJoined joined:

                    // Add the new player to the list of client ids
                    _clientIds = _clientIds.Add(callback.EndPoint, _clientIdIndex);

                    // Send a Welcome datagram to the new client, with the server's
                    // current state
                    callback.SendToCaller(
                        new Welcome
                        {
                            Snapshot = new DataModel<StateSnapshot>
                            (
                                CallerId: _clientIdIndex,
                                Secret: "Secret",
                                Value: _state.GetSnapshot()
                            )
                        }.CreateString(), true
                    );

                    // Increment the client Id index
                    ++_clientIdIndex;                                   break;

                case ClientMovement pinged: 

                    // Pinged messages can simply be forwarded to the other clients
                    callback.SendToOthers(callback.Data, false);        break;

                case Planted planted: 

                    // Attempt to add the new plant
                    var plantOption = _state.TryAddPlant(planted.Placement.Value);

                    // If successful, send the update to all clients
                    if(plantOption.IsSome(out var plant))
                    {
                        callback.SendToAll((
                            planted with {
                                Placement = planted.Placement with {
                                    Value = plant
                                } 
                            }
                        ).CreateString(), true);    
                    }                                                   break;

                case Pinged pinged:

                    callback.SendToCaller(Pinged.CreateString(), false);        break;

                default: return;
            };
        }
    }
}