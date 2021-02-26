using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class Destroyed : NetworkEvent
    {
        public DataModel<IdModel> PlantIndex { get; set; }

        public Destroyed() => PlantIndex = null;
        public Destroyed(string value) 
        {
            string [] args = value.Split('#');
            PlantIndex = new DataModel<IdModel>
            {
                CallerId = int.Parse(args[0]),
                Secret = args[1],
                Value = new IdModel { Id = uint.Parse(args[2]) }
            };
        }

        public string CreateString() => $"Destroyed::{PlantIndex.Serialize()}";
    }
}