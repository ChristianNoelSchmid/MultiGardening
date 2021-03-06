using Server.Models;

namespace Server.Networking.NetworkEvents {
    public record CritterPlacement(int Id, int Type, GridPosition Position) : ISerializable {
        public string Serialize() =>
            $"{Id}#{Type}#{Position.Serialize()}";
    }
}