namespace Server.Models
{
    public record GridPosition : ISerializable
    {
        public int X { get; init; }
        public int Y { get; init; }

        public string Serialize() =>
            string.Join('#', X, Y);
    }
}