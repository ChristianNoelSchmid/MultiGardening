namespace Server.Models
{
    public class GridPosition : ISerializable
    {
        public int X { get; set; }
        public int Y { get; set; }

        public string Serialize() =>
            string.Join("#", X, Y);
    }
}