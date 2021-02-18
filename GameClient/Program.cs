using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameClient
{
    class Program
    {
        private const int listeningPort = 3000;

        private static void StartBroadcast() 
        {
            UdpClient broadcaster = new UdpClient("127.0.0.1", listeningPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listeningPort);

            while(true)
            {
                var guid = Encoding.ASCII.GetBytes(Guid.NewGuid().ToString());
                broadcaster.Send(guid, guid.Length);
            } 
        }

        private static void Main(string[] args) => StartBroadcast();
    }
}
