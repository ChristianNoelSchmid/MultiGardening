using Newtonsoft.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class Tilled : NetworkEvent 
    {
        public DataModel<GridPosition> Position { get; set; }

        public Tilled() => Position = null;
        public Tilled(string value) =>
            Position = JsonConvert.DeserializeObject<DataModel<GridPosition>>(value);
    }
}