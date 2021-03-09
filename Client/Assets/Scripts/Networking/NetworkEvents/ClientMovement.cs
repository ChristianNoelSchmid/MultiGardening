using System;
using Server.Models;

namespace Server.Networking.NetworkEvents 
{
    /// <summary>
    /// NetworkEvent representing a Client which has
    /// supplied the Server with GridPosition data
    /// </summary>
    public class ClientMovement : NetworkEvent 
    {
        // The Client's info, with the position
        public DataModel<GridPosition> CallerInfo { get; set; }
        public ClientMovement() => CallerInfo = null;
        public ClientMovement(string value) 
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

        public string CreateString() => $"ClientMovement::{CallerInfo.Serialize()}";
    }
}