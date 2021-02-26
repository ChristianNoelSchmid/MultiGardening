using System;
using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Pinged : NetworkEvent
    {
        public DataModel<ActorMovement> CallerInfo;

        public Pinged() => CallerInfo = null;
        public Pinged(string value)
        {
            string [] args = value.Split('#');
            CallerInfo = new DataModel<ActorMovement>
            {
                CallerId = int.Parse(args[0]),
                Secret = args[1],
                Value = new ActorMovement
                {
                    Position = Tuple.Create(float.Parse(args[2]), float.Parse(args[3])),
                    IsFlipped = bool.Parse(args[4])
                } 
            };
        }

        public string CreateString() => $"Pinged::{CallerInfo.Serialize()}";
    }
}