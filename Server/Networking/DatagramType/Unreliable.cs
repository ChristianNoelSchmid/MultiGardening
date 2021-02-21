namespace Server.Networking.Datagrams
{
    public record Unreliable : Datagram
    {
        private string Message { get; init; }
        public Unreliable(string msg) => Message = msg;
    }
}