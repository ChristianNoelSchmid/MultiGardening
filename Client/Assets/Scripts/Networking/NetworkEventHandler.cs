using Server.Networking.NetworkEvents;
using System;

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
        private NetworkEvent ParseEvent(string text)
        {
            var args = text.Split(new string[] { "::" }, StringSplitOptions.None); 
            
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
        public void TransferEvent(DatagramCallback callback)
        {
            switch (ParseEvent(callback.Data))
            {
                case Pinged pinged: callback.SendToOthers(callback.Data, false); break;
                default: return;
            };
        }
    }
}