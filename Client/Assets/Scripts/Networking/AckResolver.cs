using System.Net;

namespace Server.Networking
{
    public class AckResolver
    {
        public ulong AckIndex { get; set; }
        public long TicksStart { get; set; }
        public string Message { get; set; }
    }
}