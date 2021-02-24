using Newtonsoft.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class Planted : NetworkEvent 
    {
        public DataModel<PlantPlacement> Placement { get; set; }

        public Planted() => Placement = null;
        public Planted(string value) =>
            Placement = JsonConvert.DeserializeObject<DataModel<PlantPlacement>>(value);
    }
}