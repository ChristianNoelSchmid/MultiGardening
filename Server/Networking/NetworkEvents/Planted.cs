using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Planted : NetworkEvent 
    {
        public DataModel<PlantPlacement> Placement { get; init; }

        public Planted() => Placement = null;
        public Planted(string value) =>
            Placement = JsonSerializer.Deserialize<DataModel<PlantPlacement>>(value);

        public string CreateString() => $"Planted::{JsonSerializer.Serialize(this)}";
    }
}