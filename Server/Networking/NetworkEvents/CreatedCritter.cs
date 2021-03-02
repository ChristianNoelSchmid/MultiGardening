using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record CreatedCritter : NetworkEvent
    {
        public CritterPlacement Placement { get; init; }

        public CreatedCritter() => Placement = null;

        public CreatedCritter(string data)
        {
            string [] args = data.Split("#");
            Placement = new CritterPlacement
            {
                Index = uint.Parse(args[0]),
                Type = uint.Parse(args[1]),
                Position = new GridPosition
                {
                    X = int.Parse(args[2]),
                    Y = int.Parse(args[3])
                }
            };
        }

        public string CreateString() => $"CreatedCritter{Placement.Serialize()}";
    }
}