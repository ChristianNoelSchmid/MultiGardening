using Newtonsoft.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class PlayerJoined : NetworkEvent 
    { 
        public ActorMovement ActorMovement { get; set; }

        public PlayerJoined() => ActorMovement = null;
        public PlayerJoined(string value) => 
            ActorMovement = JsonConvert.DeserializeObject<ActorMovement>(value);
    }
}