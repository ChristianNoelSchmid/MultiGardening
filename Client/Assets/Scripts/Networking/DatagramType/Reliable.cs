using System.Linq;

namespace Server.Networking.Datagrams
{
    public class Reliable : Datagram
    {
        public ulong AckIndex { get; set; }
        public string Data { get; set; }

        public Reliable(string datagram)
        {
            AckIndex = ulong.Parse(datagram.Split(new string[] { "::" }, System.StringSplitOptions.None).Last().Substring(3));
            Data = datagram.Substring(0, datagram.LastIndexOf("::"));
        }

        public static string CreateString(ulong ackIndex, string data) => 
            $"{data}::REL{ackIndex}";
    }
}