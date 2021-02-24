using Newtonsoft.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class Pinged : NetworkEvent
    {
        public DataModel<ActorMovement> CallerInfo;

        public Pinged() => CallerInfo = null;
        public Pinged(string value) => 
            CallerInfo = JsonConvert.DeserializeObject<DataModel<ActorMovement>>(value);
    }
}