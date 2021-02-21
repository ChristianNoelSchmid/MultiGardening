using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.Networking.Datagrams;

namespace Server.Networking
{
    public class NetworkDatagramHandler
    {
        /// <summary>
        /// Length of ticks for a timeout
        /// </summary>
        /// <returns>Length of a network timeout - 1 second</returns>
        private readonly double timeout = Math.Pow(10.0, 7.0); 

        /// <summary>
        /// The client for the UDP network. Handles sending and receiving
        /// of datagrams.
        /// </summary>
        private readonly UdpClient _client;

        /// <summary>
        /// Dictionary which contains all information about needed acknoledgements.
        /// Ensures specific messages are sent. When an ensured message is sent,
        /// it's ID (ulong) is stored in this Dictionary, as well as the time at
        /// which it was sent. Periodically, this Dictionary is checked and, upon
        /// a time which a values been in it longer than a specified timeout,
        /// it resends the message. Acknowledgements that do come remove the
        /// value from the Dictionary.
        /// </summary>
        private ConcurrentDictionary<IPEndPoint, List<AckResolver>> _ackResolvers;

        /// <summary>
        /// A dictionary which holds a collection of client IPEndPoints,
        /// and an index representing the total number of ensured datagrams 
        /// sent. This is updated on both the client side and distant client side,
        /// To ensure that datagrams which need acknowledgements are not lost 
        /// in the network.
        /// </summary>
        private Dictionary<IPEndPoint, ulong> _ackRemoteToLocalIndices;

        /// <summary>
        /// Represents the indices of the local client's current index count
        /// of ack datagrams, per connection. A dictionary is needed as the local
        /// client may send different amounts of ensured datagrams to each client.
        /// </summary>
        private Dictionary<IPEndPoint, ulong> _ackLocalToRemoteIndices;

        public EventHandler<string> MessageRecieved;

        public NetworkDatagramHandler(int port, string extHostname=null)
        {
            if(extHostname != null)
                _client = new UdpClient(extHostname, port);
            else 
                _client = new UdpClient(port);

            _ackResolvers = new ConcurrentDictionary<IPEndPoint, List<AckResolver>>();
        }

        public void Start()
        {
            new Thread(StartAckListener).Start();
            new Thread(StartReceiving).Start();
        }

        /// <summary>
        /// Sends a datagram to the specified end points, either reliable or unreliable.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="isReliable">Whether the handler should ensure 
        /// the message reaches its intended recipient</param>
        /// <param name="endPoints">The client(s) to send the datagram to</param>
        public void SendMessage(string message, bool isReliable, params IPEndPoint[] endPoints)
        {
            foreach(var endPoint in endPoints)
            {
                byte[] msgBytes;

                if(isReliable)
                {
                    // Update the _ackLocalToRemoteCounts to reflect the new
                    // # of reliable messages sent to specific client (+ 1)
                    ulong ackIndex = 0;
                    _ackLocalToRemoteIndices.TryGetValue(endPoint, out ackIndex);
                    _ackLocalToRemoteIndices.AddOrUpdate(endPoint, ackIndex + 1, (_,_) => ackIndex + 1);

                    // Append the ack index count to the message, to communicate to the
                    // client what reliable datagram count it represents
                    msgBytes = Encoding.ASCII.GetBytes(message + $"::REL{ ackIndex }");

                    // Create a new AckResolver, which will
                    // resend the datagram if the timeout is reached
                    // before receiving an ACK
                    var newAck = new AckResolver
                    {
                        AckIndex = ackIndex,
                        IPEndPoint = endPoint,
                        TicksStart = DateTime.Now.Ticks,
                        Message = message
                    };

                    // Retrieve the AckResolver List (if it exists) for
                    // the specified connection, and add the new AckResolver
                    // to the List
                    List<AckResolver> ackList;
                    if(!_ackResolvers.TryGetValue(endPoint, out ackList))
                        ackList = new List<AckResolver>();

                    ackList.Add(newAck);

                    _ackResolvers.AddOrUpdate(endPoint, ackList, (_, _) => ackList);
                }
                else
                    msgBytes = Encoding.ASCII.GetBytes(message);

                _client.Send(msgBytes, msgBytes.Length, endPoint);
            }
        }

        /// <summary>
        /// Sends a datagram with the specified AckResolver info
        /// As AckResolvers are built for reliable messaging, isReliable
        /// is set to true
        /// </summary>
        /// <param name="resolver">The AckResolver with the datagram info</param>
        private void SendMessage(AckResolver resolver) =>
            SendMessage(resolver.Message, true, resolver.IPEndPoint);
        private void AcceptAck(IPEndPoint endPoint, ulong ack)
        {
            List<AckResolver> resolverList;
            if(_ackResolvers.TryGetValue(endPoint, out resolverList))
                resolverList.Remove(
                    resolverList.Where(res => res.AckIndex == ack).First()
                );
        }

        private Datagram ParseDatagram(byte[] datagram)
        {
            string msg = Encoding.ASCII.GetString(datagram);
            string suffix = msg.Split("::").Last().Substring(0, 3);

            return suffix switch
            {
                "ACK" => new Ack(msg),
                "REL" => new Reliable(msg),
                "RES" => new Resend(),
                _     => new Unreliable(msg)
            };
        }


        #region Multithreaded Sub-processes
        private void StartReceiving()
        {
            IPEndPoint endPoint = null;
            ulong ackExpected = 0;
            byte[] bytes;
            Datagram datagram;

            while(true)
            {
                bytes = _client.Receive(ref endPoint);
                datagram = ParseDatagram(bytes);
                _ackRemoteToLocalIndices.TryGetValue(endPoint, out ackExpected);

                switch(datagram)
                {
                    case Reliable rel when rel.AckIndex > ackExpected:
                        SendMessage(Resend.CreateString(), false, endPoint);
                        break;
                    case Reliable rel when rel.AckIndex < ackExpected:
                        // Send ack - already retrieved
                        return;
                    case Reliable rel:
                        _ackRemoteToLocalIndices[endPoint] = ackExpected + 1;
                        MessageRecieved.Invoke(null, msg);
                        // Send ack
                        break;
                    case Resend:
                        // Forward Packets
                        break;
                    case Ack ack:
                        AcceptAck(endPoint, ack.AckIndex);
                        break;
                    case Unreliable unrel:
                        // ...
                        break; 
                }

            }
        }

        private void StartAckListener()
        {
            while(true)
            {
                Thread.Sleep(100);

                foreach(var pair in _ackResolvers)
                {
                    foreach(var resolver in pair.Value)
                    {
                        if(DateTime.Now.Ticks - resolver.TicksStart > timeout)
                        {
                            pair.Value.Remove(resolver);
                            _ackResolvers.AddOrUpdate(pair.Key, pair.Value, (_, _) => pair.Value);

                            SendMessage(resolver);
                        }
                    }
                }
            }
        }
        #endregion
    }
}