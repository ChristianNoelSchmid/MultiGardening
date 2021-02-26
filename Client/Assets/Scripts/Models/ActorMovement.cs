using System;

namespace Server.Models
{
    public class ActorMovement : ISerializable
    {
        public Tuple<float, float> Position { get; set; }
        public bool IsFlipped { get; set; }

        public string Serialize() =>
            string.Join("#", Position.Item1, Position.Item2, IsFlipped);
    }
}