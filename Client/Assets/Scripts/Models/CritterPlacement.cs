using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class CritterPlacement : ISerializable
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public GridPosition Position { get; set; }
        public string Serialize() =>
            $"{Id}#{Type}#{Position.Serialize()}";
    }
}