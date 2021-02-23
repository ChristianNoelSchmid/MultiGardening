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
        static NetworkDatagramHandler handler1 = new NetworkDatagramHandler(2000);
        static NetworkDatagramHandler handler2 = new NetworkDatagramHandler(2001);
        static NetworkDatagramHandler handler3 = new NetworkDatagramHandler(2002);
        static IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 2000);
        static void Main(string[] args)
        {
            int count = 0;
            handler1.MessageRecieved += (_, callback) => {
                Console.WriteLine($"handler1 recieved message! Contents: {callback.Data}");
            };
            new Thread(Thread1){IsBackground=true}.Start();
            //new Thread(Thread2){IsBackground=true}.Start();
            while(true);
        }

        static void Thread1()
        {
            int count = 0;
            while(true)
            {
                handler2.SendMessage($"Coffee {++count}!", true, endPoint);
            }
        }

        static void Thread2()
        {
            while(true)
            {
                handler3.SendMessage("Tea!", true, endPoint);
            }
        }
    }
}
