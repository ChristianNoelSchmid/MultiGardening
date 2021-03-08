using System;
using System.Net;

namespace Server.Networking
{
    /// <summary>
    /// Record that's forwarded to the NetworkEventHandler from
    /// the NetworkDatagramHandler, which contains information about
    /// the event, and methods to send back information to Clients
    /// </summary>
    /// <param name="Data">The event data</param>
    /// <param name="EndPoint">The Client's endpoint</param>
    /// <param name="SendToCaller">Function which send's data back to the Client who sent the datagram</param>
    /// <param name="SendToOthers">Function which send's data back to all Clients except the sender</param>
    /// <param name="SendToAll">Function which send's data back to all Clients</param>
    /// <returns></returns>
    public record DatagramCallback (
        string Data, IPEndPoint EndPoint,
        Action<string, bool> SendToCaller,
        Action<string, bool> SendToOthers,
        Action<string, bool> SendToAll
    );
}