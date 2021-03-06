using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record PlayerLeft : NetworkEvent 
    {
        public DataModel CallerInfo { get; init; }

        public PlayerLeft() => CallerInfo = null;
        public PlayerLeft(string value) 
        {
            string [] args = value.Split('#');
            CallerInfo = new DataModel (
                CallerId: int.Parse(args[0]),
                Secret: args[1]
            );
        }

        public string CreateString() => $"PlayerLeft::{CallerInfo.Serialize()}";
    }
}