namespace Server.Networking.Datagrams
{
    public class Unreliable : Datagram
    {
        public string Data { get; set; }
        public Unreliable(string data) => Data = data;

        public static string CreateString(string data) => data;
    }
}