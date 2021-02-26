using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class Planted : NetworkEvent 
    {
        public DataModel<PlantPlacement> Placement { get; set; }

        public Planted() => Placement = null;
        public Planted(string value) 
        {
            string [] args = value.Split('#');
            Placement = new DataModel<PlantPlacement>
            {
                CallerId = int.Parse(args[0]),
                Secret = args[1],
                Value = new PlantPlacement
                {
                    GridStart = new GridPosition
                    {
                        X = int.Parse(args[2]),
                        Y = int.Parse(args[3])
                    },
                    GridEnd = new GridPosition
                    {
                        X = int.Parse(args[4]),
                        Y = int.Parse(args[5])
                    },
                    PlantType = uint.Parse(args[6])
                }
            };
        }

        public string CreateString() => $"Planted::{Placement.Serialize()}";
    }
}