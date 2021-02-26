using System;
using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record PlayerJoined : NetworkEvent 
    { 
        public ActorMovement ActorMovement { get; init; }

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