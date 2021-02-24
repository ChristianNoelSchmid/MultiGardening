using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public record Pinged : NetworkEvent
    {
        public DataModel<ActorMovement> CallerInfo;

        public Pinged() => CallerInfo = null;
        public Pinged(string value) => 
            CallerInfo = JsonSerializer.Deserialize<DataModel<ActorMovement>>(value);
    }
}