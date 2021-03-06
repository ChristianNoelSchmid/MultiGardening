using System;
using System.Net;

namespace Server.Networking
{
    public record DatagramCallback (
        string Data, IPEndPoint EndPoint,
        Action<string, bool> SendToCaller,
        Action<string, bool> SendToOthers,
        Action<string, bool> SendToAll
    );
}