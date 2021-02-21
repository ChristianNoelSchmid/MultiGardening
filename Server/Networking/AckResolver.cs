using System.Net;

namespace Server.Networking
{
    public record AckResolver
    {
        public ulong AckIndex { get; init; }
        public IPEndPoint IPEndPoint { get; init; }
        public long TicksStart { get; init; }
        public string Message { get; init; }
    }
}