using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class CritterPlacement : ISerializable
    {
        public uint Index { get; set; }
        public uint Type { get; set; }
        public GridPosition Position { get; set; }
        public string Serialize() =>
            $"{Index}#{Type}#{Position.Serialize()}";
    }
}