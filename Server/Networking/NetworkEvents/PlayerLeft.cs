using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record PlayerLeft : NetworkEvent 
    {
        public DataModel CallerInfo { get; init; }

        public PlayerLeft() => CallerInfo = null;
        public PlayerLeft(string value) =>
            CallerInfo = JsonSerializer.Deserialize<DataModel>(value);
    }
}