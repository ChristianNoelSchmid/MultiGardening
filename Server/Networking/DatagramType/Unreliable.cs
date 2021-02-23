namespace Server.Networking.Datagrams
{
    public record Unreliable : Datagram
    {
        public string Data { get; init; }
        public Unreliable(string data) => Data = data;

        public static string CreateString(string data) => data;
    }
}