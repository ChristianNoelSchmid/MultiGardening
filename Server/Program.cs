using System;
using Server.Networking;
using Server.State;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var datagramHandler = new NetworkDatagramHandler(3000);
            var state = new ServerState(datagramHandler);
            var eventHandler = new NetworkEventHandler(datagramHandler, state);

            datagramHandler.MessageRecieved += (_, callback) =>
                eventHandler.TransferEvent(callback);

            while (true) ;
        }
    }
}
