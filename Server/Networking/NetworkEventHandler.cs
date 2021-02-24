using Server.Models;
using Server.Networking.NetworkEvents;
using System;
using System.Collections.Immutable;
using System.Text.Json;

namespace Server.Networking
{
    public class NetworkEventHandler
    {
        private ImmutableHashSet<int> _clientIds;

        private int _clientId = 0;

        public NetworkEventHandler() => _clientIds = ImmutableHashSet<int>.Empty;

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
                "Tilled" => new Tilled(args[1]),
                "Planted" => new Planted(args[1]),
                "Destroyed" => new Destroyed(args[1]),
                "Pinged" => new Pinged(args[1]),
                _ => null
            };
        }
        public void TransferEvent(DatagramCallback callback)
        {
            switch (ParseEvent(callback.Data))
            {
                case PlayerJoined joined:
                    _clientIds = _clientIds.Add(_clientId);

                    callback.SendToCaller(
                        new Welcome
                        {
                            DataModel = new DataModel
                            {
                                CallerId = _clientId,
                                Secret = "Secret"
                            }
                        }.CreateString(), true
                    );

                    ++_clientId;
                    break;
                case Pinged pinged: callback.SendToOthers(callback.Data, false); break;
                default: return;
            };
        }
    }
}