using System;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    /// <summary>
    /// NetworkEvent representing a Client which has
    /// pinged the Server with GridPosition data
    /// </summary>
    public class Pinged : NetworkEvent
    {
        public DataModel<GridPosition> CallerInfo { get; set; }

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