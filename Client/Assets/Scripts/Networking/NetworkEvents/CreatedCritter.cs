using System;

using Server.Models;

namespace Server.Networking.NetworkEvents
{
    /// <summary>
    /// NetworkEvent representing a new critter that has
    /// been created by the Server, to be sent to the client
    /// </summary>
    public class CreatedCritter : NetworkEvent
    {
        public CritterPlacement Placement { get; set; }

        public CreatedCritter() => Placement = null;

        public CreatedCritter(string data)
        {
            string [] args = data.Split('#');
            Placement = new CritterPlacement
            {
                Id = int.Parse(args[0]),
                Type = int.Parse(args[1]),
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