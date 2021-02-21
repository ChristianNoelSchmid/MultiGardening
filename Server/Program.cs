using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server.Networking;

namespace Server
{
    class Program
    {
        private const int listeningPort = 3000;
        private static async Task StartListener()
        {
            UdpClient listener = new UdpClient(listeningPort);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listeningPort);

            try 
            {
                while(true)
                {
                    Console.WriteLine("Waiting for broadcast...");
                    var result = await listener.ReceiveAsync();
                    var bytes = result.Buffer;

                    Console.WriteLine($"Receiving broadcast from {groupEP}");
                    Console.WriteLine(Encoding.ASCII.GetString(bytes, 0, bytes.Length));
                }
            }
            catch (SocketException e) { Console.WriteLine(e); }
            finally { listener.Close(); }
        }
        static void Main(string[] args)
        {
            StartListener().GetAwaiter().GetResult();
        }
    }
}
