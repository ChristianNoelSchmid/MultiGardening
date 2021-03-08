using Server.Models;

namespace Server.Networking.NetworkEvents 
{
    /// <summary>
    /// NetworkEvent representing that the Server
    /// has moved a critter from one GridPosition to another,
    /// and that the Client needs to update with that info.
    /// </summary>
    public record MovedCritter : NetworkEvent 
    {
        // The new placement for the critter
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