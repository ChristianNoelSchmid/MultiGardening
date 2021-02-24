namespace Server.Networking.Datagrams
{
    public class Resend : Datagram
    {
        public static string CreateString() => "RES";
    }
}