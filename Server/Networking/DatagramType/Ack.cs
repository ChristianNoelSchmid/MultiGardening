using System.Linq;

namespace Server.Networking.Datagrams
{
    public record Ack : Datagram
    {
        public ulong AckIndex { get; init; }
        public Ack(string datagram) =>
            AckIndex = ulong.Parse(datagram.Split("::").Last().Substring(3));

        public static string CreateString(ulong ackIndex) => $"::ACK{ackIndex}";
    }
}