using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server.Networking.Datagrams;
using Server.Networking.Resolvers;

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
        private readonly AckResolver _ackResolver;

        /// <summary>
        /// The client for the UDP network. Handles sending and receiving
        /// of datagrams.
        /// </summary>
        private readonly UdpClient _client;

        private readonly object _listLock = new object();

        /// <summary>
        /// A dictionary which holds a collection of client IPEndPoints,
        /// and an index representing the total number of ensured datagrams 
        /// sent. This is updated on both the client side and distant client side,
        /// To ensure that datagrams which need acknowledgements are not lost 
        /// in the network.
        /// </summary>
        private Dictionary<IPEndPoint, ulong> _ackExpectedIndices;
        private object _expectedLock = new object();

        /// <summary>
        /// Represents the indices of the local client's current index count
        /// of ack datagrams, per connection. A dictionary is needed as the local
        /// client may send different amounts of ensured datagrams to each client.
        /// </summary>
        private Dictionary<IPEndPoint, ulong> _ackCurrentIndices;

        private Dictionary<IPEndPoint, DateTime> _lastMessages;

        public EventHandler<DatagramCallback> MessageRecieved;
        public EventHandler<DatagramCallback> LostConnection;
        public bool IsListening { get; set; } = true;

        /// <summary>
        /// Creates the NetworkDatagramHandler. This can be either a client
        /// (sending an recieving data from only one source - the server), or
        /// a server (sending and recieving data from and to multiple clients)
        /// </summary>
        /// <param name="port">The port which is being connected to</param>
        /// <param name="extHostname">The server's hostname
        /// Setting this value will result in a client handler</param>
        public NetworkDatagramHandler(in int port)
        {
            _client = new UdpClient(port);
            _ackResolver = new AckResolver();
            _ackResolver.AckTimedOut += (_, ackData) => SendMessage(ackData);

            _ackCurrentIndices = new Dictionary<IPEndPoint, ulong>();
            _ackExpectedIndices = new Dictionary<IPEndPoint, ulong>();
            _lastMessages = new Dictionary<IPEndPoint, DateTime>();

            // Start the AckResolver and Begin recieving
            new Thread(CheckClientDisconnections){ IsBackground = true }.Start();
            new Thread(StartReceiving){ IsBackground = true }.Start();
        }

        /// <summary>
        /// Generically sends a datagram to the specified end points, either reliable or unreliable.
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="isReliable">Whether the handler should ensure 
        /// the message reaches its intended recipient</param>
        /// <param name="endPoints">The client(s) to send the datagram to</param>
        public void SendMessage(in string message, in bool isReliable, params IPEndPoint[] endPoints)
        {
            ulong ackIndex;
            byte[] msgBytes; 

            foreach(var endPoint in endPoints)
            {
                if(isReliable)
                {
                    // Since this code is read-only, it doesn't need to be locked
                    if(_ackResolver.IsClientBufferFull(endPoint)) continue;

                    // Update the _ackLocalToRemoteCounts to reflect the new
                    // # of reliable messages sent to specific client (+ 1)
                    if(!_ackCurrentIndices.TryGetValue(endPoint, out ackIndex))
                        ackIndex = 0;

                    // Append the ack index count to the message, to communicate to the
                    // client what reliable datagram count it represents
                    msgBytes = Encoding.ASCII.GetBytes(Reliable.CreateString(ackIndex, message));

                    // Add a new AckResolver to the resolver buffer, which will
                    // resend the datagram if the timeout is reached
                    // before receiving an ACK
                    _ackResolver.AddResolver(
                        new (
                            AckIndex: ackIndex,
                            IPEndPoint: endPoint,
                            TicksStart: DateTime.Now.Ticks,
                            Message: message
                        )
                    );

                    _ackCurrentIndices[endPoint] = ackIndex + 1;
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
        private void SendMessage(in AckResolverData resolver)
        {
            byte[] msgBytes;

            // Append the ack index count to the message, to communicate to the
            // client what reliable datagram count it represents
            msgBytes = Encoding.ASCII.GetBytes(Reliable.CreateString(resolver.AckIndex, resolver.Message));
            _client.Send(msgBytes, msgBytes.Length, resolver.IPEndPoint);
        }

        public void SendToAll(string data, bool isReliable) => 
            SendMessage(data, isReliable, _ackExpectedIndices.Keys.ToArray());

        /// <summary>
        /// Converts an incoming datagram into the correct
        /// datagram type, based on which tag is attached
        /// to the message.
        /// </summary>
        /// <param name="datagram">The sequence of bytes representing the datagram message</param>
        /// <returns></returns>
        private Datagram ParseDatagram(in byte[] datagram)
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


        #region Multithreadded Methods

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

                lock(_listLock)
                {
                    _lastMessages[endPoint] = DateTime.UtcNow;
                }
                
                if(!IsListening) continue;

                datagram = ParseDatagram(bytes);

                lock(_expectedLock)
                {
                    _ackExpectedIndices.TryGetValue(endPoint, out ackExpected);

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
                            _ackExpectedIndices[endPoint] = ackExpected + 1;
                            SendMessage(Ack.CreateString(rel.AckIndex), false, endPoint);
                            MessageRecieved.Invoke(null, new DatagramCallback (
                                Data: rel.Data,
                                EndPoint: endPoint,

                                SendToCaller: (data, isRel) => SendMessage(data, isRel, endPoint),
                                SendToOthers: (data, isRel) => SendMessage(
                                        data, isRel, _ackExpectedIndices
                                            .Keys.Where(k => k != endPoint).ToArray()
                                        ),
                                SendToAll: (data, isRel) => SendMessage(data, isRel, _ackExpectedIndices
                                            .Keys.ToArray()
                            )));                                                            break;

                        // Message contains request to resend packages
                        // Resend earliest package
                        case Resend: _ackResolver.ResendRel(endPoint);                                   break;

                        // Message is an acknowledgement datagram
                        // Accept and remove awaiting AckResolver
                        case Ack ack: _ackResolver.AcceptAck(endPoint, ack.AckIndex);                    break;

                        // Message is unreliable - invoke MessageRecieved event
                        case Unreliable unrel: 
                            MessageRecieved.Invoke(
                                null, new DatagramCallback (
                                    Data: unrel.Data,
                                    EndPoint: endPoint,

                                    SendToCaller: (data, isRel) => SendMessage(data, isRel, endPoint),
                                    SendToOthers: (data, isRel) => SendMessage(
                                            data, isRel, _ackExpectedIndices
                                                .Keys.Where(k => k != endPoint).ToArray()
                                            ),
                                    SendToAll: (data, isRel) => SendMessage(data, isRel, _ackExpectedIndices
                                                .Keys.ToArray()
                            )));                                                             break; 
                    }
                }
            }
        }

        private void CheckClientDisconnections()
        {
            while(true)
            {
                lock(_listLock)
                {
                    foreach(var pair in _lastMessages)
                    {
                        if(DateTime.UtcNow - pair.Value > TimeSpan.FromSeconds(7.5f))
                        {
                            LostConnection.Invoke(null, new DatagramCallback (
                            Data: string.Empty,
                            EndPoint: pair.Key,

                            SendToCaller: (data, isRel) => SendMessage(data, isRel, pair.Key),
                            SendToOthers: (data, isRel) => SendMessage(
                                data, isRel, _ackExpectedIndices
                                    .Keys.Where(k => k != pair.Key).ToArray()
                            ),
                            SendToAll: (data, isRel) => SendMessage(data, isRel, _ackExpectedIndices
                                .Keys.ToArray()
                            )));
                                
                            lock(_expectedLock) { _ackExpectedIndices.Remove(pair.Key); }
                            _ackResolver.RemoveClientEndPoint(pair.Key); 
                            _lastMessages.Remove(pair.Key);
                        }
                    }
                }

                Thread.Sleep(100);
            }
        }
        #endregion
    }
}