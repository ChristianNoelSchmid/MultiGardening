namespace Server.Networking.Datagrams
{
    /// <summary>
    /// Sends a simple datagram, which will not
    /// be resent if droppped.
    /// </summary>
    public record Unreliable : Datagram
    {
        public string Data { get; init; }
        public Unreliable(string data) => Data = data;

        public static string CreateString(string data) => data;
    }
}