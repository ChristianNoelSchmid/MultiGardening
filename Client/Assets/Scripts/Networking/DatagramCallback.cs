using System;

namespace Server.Networking
{
    public class DatagramCallback
    {
        public string Data { get; set; }
        public Action<string, bool> SendToServer { get; set; }
    }
}