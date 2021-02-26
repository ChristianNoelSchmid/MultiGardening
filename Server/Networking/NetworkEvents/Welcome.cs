using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Welcome : NetworkEvent 
    {
        public DataModel DataModel { get; init; }
        public Welcome() => DataModel = null;
        public Welcome(string value) 
        {
            string [] args = value.Split('#');
            DataModel = new DataModel
            {
                CallerId = int.Parse(args[0]),
                Secret = args[1]
            };
        }

        public string CreateString() => $"Welcome::{DataModel.Serialize()}";
    }
}