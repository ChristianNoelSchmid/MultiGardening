using System;
using Server.Networking;

namespace Server.Networking
{
    public static class NetworkEventParser
    {
        public static Event ParseNetworkEvent(string text)
        {
            var args = text.Split("::"); 
            
            switch(args[0])
            {
                case "PlayerJoined":
                    return new PlayerJoined(args[1]);
                case "PlayerLeft":
                    return new PlayerLeft(args[1]);                
                case "Tilled":
                    return new Tilled(args[1]);
                case "Destroyed":
                    return new Destroyed(args[1]);
                case "Pinged":
                    return new Pinged(args[1]);
            }

            return null;
        }
    }
}