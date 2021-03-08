using System;
using Server.Networking;
using Server.State;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the NetworkDatagramHandler, NetworkEventHandler,
            // and ServerState
            var datagramHandler = new NetworkDatagramHandler(3000);
            var state = new ServerState(datagramHandler);
            var eventHandler = new NetworkEventHandler(datagramHandler, state);

            // Loop until Ctrl-C
            while (true);
        }
    }
}
