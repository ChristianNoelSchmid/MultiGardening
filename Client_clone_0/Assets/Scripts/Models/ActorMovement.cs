using System;

namespace Server.Models
{
    public class ActorMovement
    {
        public Tuple<float, float> Position { get; set; }
        public bool IsFlipped { get; set; }
    }
}