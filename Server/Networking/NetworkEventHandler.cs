using System.Diagnostics;
using Server.Models;
using Server.Networking.NetworkEvents;
using System.Collections.Immutable;
using System;

namespace Server.Networking
{
    public class NetworkEventHandler
    {
        private readonly float[] _plantSecondsToAdd = new float [] {
            10, 10, 15
        };
        private ImmutableHashSet<int> _clientIds;
        private State _state;

        private int _clientId = 1;

        public NetworkEventHandler() 
        {
            _clientIds = ImmutableHashSet<int>.Empty;
            _state = new State();
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
                "CreatedCritter" => new CreatedCritter(args[1]),
                "MovedCritter" => new MovedCritter(args[1]),
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

                    ++_clientId;                                    break;

                case Pinged pinged: 
                    callback.SendToOthers(callback.Data, false);    break;

                case Planted planted: 
                    if(_state.TryAddPlant(planted.Placement.Value))
                    {
                        callback.SendToAll((
                            planted with {
                                Placement = planted.Placement with {
                                    Value = planted.Placement.Value with {
                                        TimeToComplete = DateTime.UtcNow.AddSeconds
                                            (_plantSecondsToAdd[planted.Placement.Value.PlantType])
                            } } }
                        ).CreateString(), true);    
                    }                                               break;

                default: return;
            };
        }
    }
}