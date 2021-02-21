namespace Server.Models
{
    public record PlantPlacement
    {
        public GridPosition GridStart { get; init; }
        public GridPosition GridEnd { get; init; }
        public uint PlantType { get; init; }
    }
}