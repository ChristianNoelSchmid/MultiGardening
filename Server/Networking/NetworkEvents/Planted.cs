using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Planted : NetworkEvent 
    {
        public DataModel<PlantPlacement> Placement { get; init; }

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
                    Position = new GridPosition
                    {
                        X = int.Parse(args[2]),
                        Y = int.Parse(args[3])
                    },
                    PlantType = uint.Parse(args[4]),
                    FullGrownAtSeconds = double.Parse(args[5])
                }
            };
        }

        public string CreateString() => $"Planted::{Placement.Serialize()}";
    }
}