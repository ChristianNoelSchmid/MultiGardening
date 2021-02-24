using Newtonsoft.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class Destroyed : NetworkEvent
    {
        public DataModel<uint> PlantIndex { get; set; }

        public Destroyed() => PlantIndex = null;
        public Destroyed(string value) =>
            PlantIndex = JsonConvert.DeserializeObject<DataModel<uint>>(value);
        public string CreateString() => $"Destroyed::{JsonConvert.SerializeObject(this)}";
    }
}