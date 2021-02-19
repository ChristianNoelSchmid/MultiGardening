namespace Server.Models
{
    public record PlantPlacement
    {
        public GridPosition[] GridPositions { get; init; }
        public uint PlantType { get; init; }
    }
}