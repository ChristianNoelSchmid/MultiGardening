using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Server.Networking;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var datagramHandler = new NetworkDatagramHandler(3000);
            var eventHandler = new NetworkEventHandler();

            datagramHandler.MessageRecieved += (_, callback) =>
            {
                Console.WriteLine($"Recieved datagram: {callback.Data}");
                eventHandler.TransferEvent(callback);
            };

            while (true) ;
        }
    }
}
