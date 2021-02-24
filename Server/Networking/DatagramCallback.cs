using System;

namespace Server.Networking
{
    public record DatagramCallback
    {
        public string Data { get; init; }
        public Action<string, bool> SendToCaller { get; init; }
        public Action<string, bool> SendToOthers { get; init; }
        public Action<string, bool> SendToAll { get; init; }
    }
}