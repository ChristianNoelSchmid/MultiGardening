using Server.Networking.NetworkEvents;

namespace Server.Networking
{
    public class NetworkEventHandler
    {
        /// <summary>
        /// Converts incoming text into an appropriate
        /// NetworkEvent.
        /// </summary>
        /// <param name="text">The string to parse</param>
        /// <returns></returns>
        public NetworkEvent ParseEvent(string text)
        {
            var args = text.Split("::"); 
            
            return args[0] switch
            {
                "PlayerJoined" => new PlayerJoined(args[1]),
                "PlayerLeft" => new PlayerLeft(args[1]),
                "Tilled" => new Tilled(args[1]),
                "Planted" => new Planted(args[1]),
                "Destroyed" => new Destroyed(args[1]),
                "Pinged" => new Pinged(args[1]),
                _ => null
            };
        }
    }
}