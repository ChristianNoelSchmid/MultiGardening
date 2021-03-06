namespace Server.Models {
    public record GridPosition(int X, int Y) : ISerializable {
        public string Serialize() =>
            string.Join('#', X, Y);
    }
}