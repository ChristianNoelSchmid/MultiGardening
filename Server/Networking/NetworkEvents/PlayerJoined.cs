using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record PlayerJoined : NetworkEvent 
    { 
        public ActorMovement ActorMovement { get; init; }

        public PlayerJoined() => ActorMovement = null;
        public PlayerJoined(string value) => 
            ActorMovement = JsonSerializer.Deserialize<ActorMovement>(value);

        public string CreateString() => $"PlayerJoined::{JsonSerializer.Serialize(this)}";
    }
}