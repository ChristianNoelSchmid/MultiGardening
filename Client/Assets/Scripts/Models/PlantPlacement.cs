using System;

namespace Server.Models
{
    public class PlantPlacement : ISerializable
    {
        public GridPosition Position { get; set; }
        public int PlantType { get; set; }

        // Represents the total seconds (since 1970) to the point
        // when the plan
        public DateTime TimeToComplete { get; set; }

        public string Serialize() =>
            $"{Position.Serialize()}#{PlantType}#{TimeToComplete.ToString("yyyy-MM-dd HH:mm:ss")}";
    }
}