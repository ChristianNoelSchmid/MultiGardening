using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class Tilled : NetworkEvent 
    {
        public DataModel<GridPosition> Position { get; set; }
        public Tilled() => Position = null;
        public Tilled(string value) 
        {
            string [] args = value.Split('#');
            Position = new DataModel<GridPosition>
            {
                CallerId = int.Parse(args[0]),
                Secret = args[1],
                Value = new GridPosition
                {
                    X = int.Parse(args[3]),
                    Y = int.Parse(args[4])
                }
            };
        }

        public string CreateString() => $"Tilled::{Position.Serialize()}";
    }
}