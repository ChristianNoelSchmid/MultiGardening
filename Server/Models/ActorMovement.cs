using System;

namespace Server.Models
{
    public record ActorMovement
    {
        public Tuple<float, float> Position { get; init; }
        public bool IsFlipped { get; init; }
    }
}