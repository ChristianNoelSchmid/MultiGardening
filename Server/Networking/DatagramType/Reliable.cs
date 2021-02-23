using System.Linq;

namespace Server.Networking.Datagrams
{
    public record Reliable : Datagram
    {
        public ulong AckIndex { get; init; }
        public string Data { get; init; }

        public Reliable(string datagram)
        {
            AckIndex = ulong.Parse(datagram.Split("::").Last().Substring(3));
            Data = datagram.Substring(0, datagram.LastIndexOf("::"));
        }

        public static string CreateString(ulong ackIndex, string data) => 
            $"{data}::REL{ackIndex}";
    }
}