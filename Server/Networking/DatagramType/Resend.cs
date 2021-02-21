namespace Server.Networking.Datagrams
{
    public record Resend : Datagram
    {
        public static string CreateString() => "RES";
    }
}