using Server.Models;
using Server.Networking.NetworkEvents;
using Server.State;
using System.Collections.Immutable;
using System.Net;

namespace Server.Networking
{
    public class NetworkEventHandler
    {
        private ImmutableDictionary<IPEndPoint, int> _clientIds;
        private ServerState _state;

        private int _clientId = 1;

        public NetworkEventHandler(
            in NetworkDatagramHandler datagramHandler,
            in ServerState state
        ) {
            _clientIds = ImmutableDictionary<IPEndPoint, int>.Empty;
            _state = state;

            datagramHandler.LostConnection += (_, callback) => {
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
            };
        }

        /// <summary>
        /// Converts incoming text into an appropriate
        /// NetworkEvent.
        /// </summary>
        /// <param name="text">The string to parse</param>
        /// <returns></returns>
        
        private NetworkEvent ParseEvent(string text)
        {
            var args = text.Split("::"); 
            
            return args[0] switch
            {
                "PlayerJoined" => new PlayerJoined(args[1]),
                "PlayerLeft" => new PlayerLeft(args[1]),
                "Planted" => new Planted(args[1]),
                "Pinged" => new Pinged(args[1]),
                _ => null
            };
        }
        public void TransferEvent(in DatagramCallback callback)
        {
            switch (ParseEvent(callback.Data))
            {
                case PlayerJoined joined:

                    _clientIds = _clientIds.Add(callback.EndPoint, _clientId);

                    callback.SendToCaller(
                        new Welcome
                        {
                            Snapshot = new DataModel<StateSnapshot>
                            (
                                CallerId: _clientId,
                                Secret: "Secret",
                                Value: _state.GetSnapshot()
                            )
                        }.CreateString(), true
                    );

                    ++_clientId;                                    break;

                case Pinged pinged: 

                    callback.SendToOthers(callback.Data, false);    break;

                case Planted planted: 

                    var plantOption = _state.TryAddPlant(planted.Placement.Value);
                    if(plantOption.IsSome(out var plant))
                    {
                        callback.SendToAll((
                            planted with {
                                Placement = planted.Placement with {
                                    Value = plant
                                } 
                            }
                        ).CreateString(), true);    
                    }                                               break;

                default: return;
            };
        }
    }
}