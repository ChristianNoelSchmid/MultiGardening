using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Destroyed : NetworkEvent
    {
        public DataModel<uint> PlantIndex { get; init; }

        public Destroyed() => PlantIndex = null;
        public Destroyed(string value) =>
            PlantIndex = JsonSerializer.Deserialize<DataModel<uint>>(value);
    }
}