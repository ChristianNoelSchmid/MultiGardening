namespace Server.Models
{
    public class PlantPlacement : ISerializable
    {
        public GridPosition Position { get; set; }
        public uint PlantType { get; set; }

        // Represents the total seconds (since 1970) to the point
        // when the plant is fully grown. 
        public double FullGrownAtSeconds { get; set; }
        public string Serialize() =>
            $"{Position.Serialize()}#{PlantType}#{FullGrownAtSeconds}";
    }
}