using Server.Models;

namespace Server.Networking.NetworkEvents {
    public record MovedCritter : NetworkEvent {
        public CritterPlacement Placement { get; init; }

        public MovedCritter() => Placement = null;

        public MovedCritter(string data)
        {
            string [] args = data.Split("#");
            Placement = new CritterPlacement(
                Id: int.Parse(args[0]),
                Type: int.Parse(args[1]),
                Position: new GridPosition(
                    int.Parse(args[2]),
                    int.Parse(args[3])
                )
            );
        }

        public string CreateString() => $"MovedCritter::{Placement.Serialize()}";
    }
}