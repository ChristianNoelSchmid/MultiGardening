using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record CritterPlacement : ISerializable
    {
        public uint Index { get; init; }
        public uint Type { get; init; }
        public GridPosition Position { get; init; }
        public string Serialize() =>
            $"{Index}#{Type}#{Position.Serialize()}";
    }
}