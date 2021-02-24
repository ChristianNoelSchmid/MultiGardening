using Server.Models;
using Newtonsoft.Json;

namespace Server.Networking.NetworkEvents
{
    public class Welcome : NetworkEvent
    {
        public DataModel DataModel { get; set; }

        public Welcome() => DataModel = null;
        public Welcome(string value) => 
            DataModel = JsonConvert.DeserializeObject<DataModel>(value);
        public string CreateString() => $"Welcome::{JsonConvert.SerializeObject(this)}";
    }
}
