using System;
using System.Globalization;
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
                    Position = new GridPosition
                    {
                        X = int.Parse(args[2]),
                        Y = int.Parse(args[3])
                    },
                    PlantType = uint.Parse(args[4]),
                    TimeToComplete = DateTime.ParseExact (
                        args[5], "yyyy-MM-dd HH:mm:ss", 
                        CultureInfo.InvariantCulture, DateTimeStyles.None
                    )
                }
            };
        }

        public string CreateString() => $"Planted::{Placement.Serialize()}";
    }
}