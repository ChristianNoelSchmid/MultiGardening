using System;
using System.Globalization;
using Server.Models;

namespace Server.Networking.NetworkEvents 
{
    /// <summary>
    /// NetworkEvent representing that the Client wishes to
    /// plant something in the specified GridPosition.
    /// </summary>
    public record Planted : NetworkEvent 
    {
        // The Client info, with plant placement info, such as
        // GridPosition and type of plant
        public DataModel<PlantPlacement> Placement { get; init; }
        public Planted() => Placement = null;
        public Planted(string value) {

            string [] args = value.Split('#');

            Placement = new DataModel<PlantPlacement>(
                CallerId: int.Parse(args[0]),
                Secret: args[1],
                Value: new PlantPlacement (
                    Position:new GridPosition (
                        int.Parse(args[2]),
                        int.Parse(args[3])
                    ),
                    PlantType: int.Parse(args[4]),
                    TimeToComplete: DateTime.ParseExact (
                        args[5], "yyyy-MM-dd HH:mm:ss", 
                        CultureInfo.InvariantCulture, DateTimeStyles.None
            )));
        }

        public string CreateString() => $"Planted::{Placement.Serialize()}";
    }
}