using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Tilled : NetworkEvent 
    {
        public DataModel<GridPosition> Position { get; init; }

        public Tilled() => Position = null;
        public Tilled(string value) =>
            Position = JsonSerializer.Deserialize<DataModel<GridPosition>>(value);
    }
}