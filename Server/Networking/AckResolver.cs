using System.Net;

namespace Server.Networking {
    public record AckResolver(
        ulong AckIndex, IPEndPoint IPEndPoint, 
        long TicksStart, string Message
    );
}