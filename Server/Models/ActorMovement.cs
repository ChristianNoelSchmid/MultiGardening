using System;

namespace Server.Models
{
    public record ActorMovement : ISerializable
    {
        public Tuple<float, float> Position { get; init; }
        public bool IsFlipped { get; init; }

        public string Serialize() =>
            string.Join('#', Position.Item1, Position.Item2, IsFlipped);
    }
}