namespace Server.State
{
    public record PlantInfo
    {
        public uint PlantType { get; init; }
        public float SecondsToGrow { get; init; }
    }
}