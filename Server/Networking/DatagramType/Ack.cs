using System.Linq;

namespace Server.Networking.Datagrams
{
    /// <summary>
    /// Contains information related to which acknowledgement
    /// index either the Client or Server is sending / receiving.
    /// </summary>
    public record Ack : Datagram
    {
        // The index of the acknowledgement
        public ulong AckIndex { get; init; }
        public Ack(string datagram) =>
            AckIndex = ulong.Parse(datagram.Split("::").Last().Substring(3));

        public static string CreateString(ulong ackIndex) => $"::ACK{ackIndex}";
    }
}