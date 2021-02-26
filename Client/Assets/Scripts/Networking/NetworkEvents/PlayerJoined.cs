using System;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class PlayerJoined : NetworkEvent 
    { 
        public ActorMovement ActorMovement { get; set; }

        public PlayerJoined() => ActorMovement = null;
        public PlayerJoined(string value)
        {
            string [] args = value.Split('#');
            ActorMovement = new ActorMovement
            {
                Position = Tuple.Create(float.Parse(args[0]), float.Parse(args[1])),
                IsFlipped = bool.Parse(args[2])
            };
        }

        public string CreateString() => $"PlayerJoined::{ActorMovement.Serialize()}";
    }
}