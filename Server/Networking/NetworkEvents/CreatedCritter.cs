using Server.Models;

namespace Server.Networking.NetworkEvents 
{
    /// <summary>
    /// NetworkEvent representing a new critter that has
    /// been created by the Server, to be sent to the client
    /// </summary>

    public record CreatedCritter : NetworkEvent 
    {
        public CritterPlacement Placement { get; init; }
        public CreatedCritter() => Placement = null;
        public CreatedCritter(string data) 
        {

            string [] args = data.Split("#");
            Placement = new CritterPlacement(
                Id: int.Parse(args[0]),
                Type: int.Parse(args[1]),
                Position: new GridPosition(int.Parse(args[2]), int.Parse(args[3]))
            );
        }

        public string CreateString() => $"CreatedCritter::{Placement.Serialize()}";
    }
}