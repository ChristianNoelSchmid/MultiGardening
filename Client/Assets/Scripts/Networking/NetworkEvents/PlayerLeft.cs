using Server.Models;

namespace Server.Networking.NetworkEvents
{
    /// <summary>
    /// NetworkEvent, sent by Server, informing Clients
    /// that a particular Client has left the game.
    /// </summary>
 
    public class PlayerLeft : NetworkEvent 
    {
        public DataModel CallerInfo { get; set; }

        public PlayerLeft() => CallerInfo = null;
        public PlayerLeft(string value) 
        {
            string [] args = value.Split('#');
            CallerInfo = new DataModel
            {
                CallerId = int.Parse(args[0]),
                Secret = args[1]
            };
        }

        public string CreateString() => $"PlayerLeft::{CallerInfo.Serialize()}";
    }
}