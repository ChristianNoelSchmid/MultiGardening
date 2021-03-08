using System;

using Server.Models;

namespace Server.Networking.NetworkEvents
{
    /// <summary>
    /// NetworkEvent representing that the Server
    /// has moved a critter from one GridPosition to another,
    /// and that the Client needs to update with that info.
    /// </summary>
    public class MovedCritter : NetworkEvent
    {
        public CritterPlacement Placement { get; set; }

        public MovedCritter() => Placement = null;

        public MovedCritter(string data)
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

        public string CreateString() => $"MovedCritter{Placement.Serialize()}";
    }
}