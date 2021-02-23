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
    /// 
    /// <summary>
    /// Handles the close-to-metal transmission of UDP
    /// datagram packets. Handles relevant flags (such as REL, ACK
    /// and RES), and automatically handles retransmission of reliable
    /// packets, and requesting retransmissions from other UDP clients.
    /// </summary>
    /// 
    /// The handler can send messages to any arbitrary server, but it can
    /// also react to messages received by connections. This is done via
    /// an event being triggered with two callbacks - one to send a new
    /// datagram to the caller, and another to send information to all
    /// other connected clients.
    /// 
    public class NetworkDatagramHandler
    {
        /// <summary>
        /// Length of ticks for a packet timeout
        /// </summary>
        /// <returns>Length of a network timeout - 1 second</returns>
        private readonly double timeout = Math.Pow(10.0, 7.0);
        private Thread _listeningThread;
        private Thread _resolverThread;

        /// <summary>
        /// The client for the UDP network. Handles sending and receiving
        /// of datagrams.
        /// </summary>
        private readonly UdpClient _client;
        private IPHostEntry _hostEntry;

        /// <summary>
        /// Dictionary which contains all information about needed acknoledgements.
        /// Ensures specific messages are sent. When an ensured message is sent,
        /// it's ID (ulong) is stored in this Dictionary, as well as the time at
        /// which it was sent. Periodically, this Dictionary is checked and, upon
        /// a time which a values been in it longer than a specified timeout,
        /// it resends the message. Acknowledgements that do come remove the
        /// value from the Dictionary.
        /// </summary>
        private ConcurrentDictionary<IPEndPoint, List<AckResolver>> _resolverBuffer;
        private object _listLock = new object();

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

        public EventHandler<DatagramCallback> MessageRecieved;
        public bool IsListening { get; set; } = true;

        /// <summary>
        /// Creates the NetworkDatagramHandler. This can be either a client
        /// (sending an recieving data from only one source - the server), or
        /// a server (sending and recieving data from and to multiple clients)
        /// </summary>
        /// <param name="port">The port which is being connected to</param>
        /// <param name="extHostname">The server's hostname
        /// Setting this value will result in a client handler</param>
        public NetworkDatagramHandler(int port)
        {
            _client = new UdpClient(port);

            _resolverBuffer = new ConcurrentDictionary<IPEndPoint, List<AckResolver>>();
            _ackLocalToRemoteIndices = new Dictionary<IPEndPoint, ulong>();
            _ackRemoteToLocalIndices = new Dictionary<IPEndPoint, ulong>();

            // Start the AckResolver
            new Thread(StartAckResolver){ IsBackground = true }.Start();
            new Thread(StartReceiving){ IsBackground = true }.Start();
        }

        /// <summary>
        /// Generically sends a datagram to the specified end points, either reliable or unreliable.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="isReliable">Whether the handler should ensure 
        /// the message reaches its intended recipient</param>
        /// <param name="endPoints">The client(s) to send the datagram to</param>
        public void SendMessage(string message, bool isReliable, params IPEndPoint[] endPoints)
        {
            List<AckResolver> ackList;
            ulong ackIndex;
            byte[] msgBytes; 

            foreach(var endPoint in endPoints)
            {
                if(isReliable)
                {
                    if(!_resolverBuffer.TryGetValue(endPoint, out ackList))
                        ackList = new List<AckResolver>();

                    if(ackList.Count >= 100) return;

                    // Update the _ackLocalToRemoteCounts to reflect the new
                    // # of reliable messages sent to specific client (+ 1)
                    if(!_ackLocalToRemoteIndices.TryGetValue(endPoint, out ackIndex))
                        ackIndex = 0;

                    // Append the ack index count to the message, to communicate to the
                    // client what reliable datagram count it represents
                    msgBytes = Encoding.ASCII.GetBytes(Reliable.CreateString(ackIndex, message));

                    // Add a new AckResolver to the resolver buffer, which will
                    // resend the datagram if the timeout is reached
                    // before receiving an ACK
                    AddAckResolver(
                        new () {
                            AckIndex = ackIndex,
                            IPEndPoint = endPoint,
                            TicksStart = DateTime.Now.Ticks,
                            Message = message
                        }
                    );

                    _ackLocalToRemoteIndices[endPoint] = ackIndex + 1;
                }
                else
                    msgBytes = Encoding.ASCII.GetBytes(Unreliable.CreateString(message));

                _client.Send(msgBytes, msgBytes.Length, endPoint);
            }
        }

        /// <summary>
        /// Sends a datagram with the specified AckResolver info
        /// The datagram must be sent reliably, but the AckResolver
        /// is already in the buffer, so there's no need to add it again.
        /// </summary>
        /// <param name="resolver">The AckResolver with the datagram info</param>
        private void SendMessage(AckResolver resolver)
        {
            byte[] msgBytes;

            // Append the ack index count to the message, to communicate to the
            // client what reliable datagram count it represents
            msgBytes = Encoding.ASCII.GetBytes(Reliable.CreateString(resolver.AckIndex, resolver.Message));
            _client.Send(msgBytes, msgBytes.Length, resolver.IPEndPoint);
        }

        /// <summary>
        /// Adds a new AckResolver to the resolver buffer
        /// </summary>
        /// <param name="resolver">The new AckResolver to add</param>
        private void AddAckResolver(AckResolver resolver)
        {
            List<AckResolver> ackList;

            // Retrieve the AckResolver List (if it exists) for
            // the specified connection, and add the new AckResolver
            // to the List
            if(!_resolverBuffer.TryGetValue(resolver.IPEndPoint, out ackList))
                ackList = new List<AckResolver>();

            lock(_listLock) { 
                ackList.Add(resolver); 
                ackList.Sort((ack1, ack2) => ack1.AckIndex.CompareTo(ack2.AckIndex));
            }

            _resolverBuffer.AddOrUpdate(resolver.IPEndPoint, ackList, (_, _) => ackList);
        }

        private void AcceptAck(IPEndPoint endPoint, ulong ack)
        {
            List<AckResolver> resolverList;
            int index;
            if(_resolverBuffer.TryGetValue(endPoint, out resolverList))
            {
                lock(_listLock)
                {
                    index = resolverList.FindIndex(res => res.AckIndex == ack);
                    if(index != -1) resolverList.RemoveAt(index);
                }
            }
        }

        /// <summary>
        /// Resends the earliest reliable datagram, using
        /// the ack resolver.
        /// </summary>
        /// <param name="endPoint">The end point to send the datagram to</param>
        private void ResendRel(IPEndPoint endPoint)
        {
            List<AckResolver> list;
            AckResolver resolver;
            if(_resolverBuffer.TryGetValue(endPoint, out list))
            {
                if((resolver = list.First()) != null)
                    SendMessage(resolver);
            }
        }

        /// <summary>
        /// Converts an incoming datagram into the correct
        /// datagram type, based on which tag is attached
        /// to the message.
        /// </summary>
        /// <param name="datagram">The sequence of bytes representing the datagram message</param>
        /// <returns></returns>
        private Datagram ParseDatagram(byte[] datagram)
        {
            string msg = Encoding.ASCII.GetString(datagram);
            string suffix = msg.Split("::").Last();

            switch(suffix)
            {
                case string s when s.StartsWith("ACK"):
                    return new Ack(msg);
                case string s when s.StartsWith("REL"):
                    return new Reliable(msg);
                case string s when s.StartsWith("RES"):
                    return new Resend();
                default:
                    return new Unreliable(msg);
            };
        }

        #region Multithreaded Sub-processes
        /// <summary>
        /// The system which handles recieving datagrams, converting
        /// them to their appropriate type, and passing basic information
        /// back to the client and / or the local application.
        /// </summary>
        private void StartReceiving()
        {
            IPEndPoint endPoint = null;
            ulong ackExpected = 0;
            byte[] bytes;
            Datagram datagram;

            while(true)
            {
                if(!IsListening) continue;

                bytes = _client.Receive(ref endPoint); 
                
                if(!IsListening) continue;

                datagram = ParseDatagram(bytes);
                _ackRemoteToLocalIndices.TryGetValue(endPoint, out ackExpected);


                switch(datagram)
                {
                    // Datagram is reliable, but the ACK index is too high.
                    // Ask client to resend awaiting data.
                    case Reliable rel when rel.AckIndex > ackExpected:
                        SendMessage(Resend.CreateString(), false, endPoint);            break;

                    // Message is reliable, but the ACK index is too low.
                    // Already recieved datagram: simply resend ACK and return
                    case Reliable rel when rel.AckIndex < ackExpected:
                        SendMessage(Ack.CreateString(rel.AckIndex), false, endPoint);   break;

                    // Message is reliable, and was the expected index.
                    // Accept message, send ACK, and invoke MessageRecieved event
                    case Reliable rel:
                        _ackRemoteToLocalIndices[endPoint] = ackExpected + 1;
                        SendMessage(Ack.CreateString(rel.AckIndex), false, endPoint);
                        MessageRecieved.Invoke(null, new DatagramCallback
                        {
                            Data = rel.Data,
                            SendToCaller = (data, isRel) => SendMessage(data, isRel, endPoint),
                            SendToOthers = (data, isRel) => SendMessage(
                                    data, isRel, _ackRemoteToLocalIndices
                                        .Keys.Where(k => k != endPoint).ToArray()
                                    )
                        });                                                             break;

                    // Message contains request to resend packages
                    // Resend earliest package
                    case Resend: ResendRel(endPoint);                                   break;

                    // Message is an acknowledgement datagram
                    // Accept and remove awaiting AckResolver
                    case Ack ack: AcceptAck(endPoint, ack.AckIndex);                    break;

                    // Message is unreliable - invoke MessageRecieved event
                    case Unreliable unrel: 
                        MessageRecieved.Invoke(null, new DatagramCallback
                        {
                            Data = unrel.Data,
                            SendToCaller = (data, isRel) => SendMessage(data, isRel, endPoint),
                            SendToOthers = (data, isRel) => SendMessage(
                                    data, isRel, _ackRemoteToLocalIndices
                                        .Keys.Where(k => k != endPoint).ToArray()
                                    )
                        });                                                             break; 
                }
            }
        }

        /// <summary>
        /// Initiates the AckListener, which will routinely send
        /// reliable messages every time its AckResolver times out
        /// in the resolver buffer.
        /// </summary>
        private void StartAckResolver()
        {
            AckResolver resolver;

            while(true)
            {
                Thread.Sleep(100);

                foreach(var list in _resolverBuffer.Values)
                {
                    resolver = list.FirstOrDefault();
                    if(DateTime.Now.Ticks - resolver?.TicksStart > timeout)
                    {
                        foreach(var res in list)
                            SendMessage(res);
                    }
                }
            }
        }
        #endregion
    }
}