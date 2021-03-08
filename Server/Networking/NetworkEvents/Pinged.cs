using System;
using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents 
{
    /// <summary>
    /// NetworkEvent representing a Client which has
    /// pinged the Server with GridPosition data
    /// </summary>
    public record Pinged : NetworkEvent 
    {
        // The Client's info, with the position
        public DataModel<GridPosition> CallerInfo;
        public Pinged() => CallerInfo = null;
        public Pinged(string value) 
        {

            string [] args = value.Split('#');
            CallerInfo = new DataModel<GridPosition> (
                CallerId: int.Parse(args[0]),
                Secret: args[1],
                Value: new GridPosition(
                    int.Parse(args[2]),
                    int.Parse(args[3])
                )
            );
        }

        public string CreateString() => $"Pinged::{CallerInfo.Serialize()}";
    }
}