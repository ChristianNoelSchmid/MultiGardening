using System;
using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Pinged : NetworkEvent
    {
        public DataModel<GridPosition> CallerInfo;

        public Pinged() => CallerInfo = null;
        public Pinged(string value)
        {
            string [] args = value.Split('#');
            CallerInfo = new DataModel<GridPosition>
            {
                CallerId = int.Parse(args[0]),
                Secret = args[1],
                Value = new GridPosition
                {
                    X = int.Parse(args[2]),
                    Y = int.Parse(args[3])
                } 
            };
        }

        public string CreateString() => $"Pinged::{CallerInfo.Serialize()}";
    }
}