using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Welcome : NetworkEvent 
    {
        public DataModel DataModel { get; init; }

        public Welcome() => DataModel = null;
        public Welcome(string value) => 
            DataModel = JsonSerializer.Deserialize<DataModel>(value);

        public string CreateString() => $"Welcome::{JsonSerializer.Serialize(this)}";
    }
}