using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    /// <summary>
    /// The abstract record for all events that can
    /// be called from parsed udp datagram data.
    /// </summary>
    public abstract record NetworkEvent { }
}