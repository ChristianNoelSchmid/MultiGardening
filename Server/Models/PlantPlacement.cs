namespace Server.Models
{
    /// <summary>
    /// All plant placement state, regarding when and where a Client
    /// places a plant. Forwarded to other clients, and used to
    /// check placement in the future, and placement of critters.
    /// </summary>
    public record PlantPlacement : ISerializable
    {
        public GridPosition Position { get; init; }
        public uint PlantType { get; init; }

        // Represents the total seconds (since 1970) to the point
        // when the plant is fully grown. 
        public double FullGrownAtSeconds { get; init; }
        public string Serialize() =>
            $"{Position.Serialize()}#{PlantType}#{FullGrownAtSeconds}";
    }
}