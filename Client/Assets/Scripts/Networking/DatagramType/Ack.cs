using System.Linq;

namespace Server.Networking.Datagrams
{
    public class Ack : Datagram
    {
        public ulong AckIndex { get; set; }
        public Ack(string datagram) =>
            AckIndex = ulong.Parse(datagram.Split(new string[] { "::" }, System.StringSplitOptions.None).Last().Substring(3));

        public static string CreateString(ulong ackIndex) => $"::ACK{ackIndex}";
    }
}