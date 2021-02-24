using Newtonsoft.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class PlayerLeft : NetworkEvent 
    {
        public DataModel CallerInfo { get; set; }

        public PlayerLeft() => CallerInfo = null;
        public PlayerLeft(string value) =>
            CallerInfo = JsonConvert.DeserializeObject<DataModel>(value);

        public string CreateString() => $"PlayerLeft::{JsonConvert.SerializeObject(this)}";
    }
}