namespace Server.Models
{
    public record PlantPlacement : ISerializable
    {
        public GridPosition GridStart { get; init; }
        public GridPosition GridEnd { get; init; }
        public uint PlantType { get; init; }

        public string Serialize() =>
            $"{GridStart.Serialize()}#{GridEnd.Serialize()}#{PlantType}";
    }
}