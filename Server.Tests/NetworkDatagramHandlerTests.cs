using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using Server.Networking;

namespace Server.Tests
{
    public class NetworkDatagramHandlerTests
    {
        private IPEndPoint _endPoint1;
        private bool _transmitted;
        private bool _cycled;
        private bool _branched;
        private int _lastRecieved;
        private NetworkDatagramHandler _handler1;
        private NetworkDatagramHandler _handler2;
        private NetworkDatagramHandler _handler3;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            int port1 = 3000;
            int port2 = 3500;
            int port3 = 4000;

            _endPoint1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port1);

            _handler1 = new(port1);
            _handler1.MessageRecieved += (_, callback) => {

                Console.WriteLine($"handler 1 recieved: { callback.Data }");
                _transmitted = true;
                _lastRecieved = callback.Data switch 
                {
                    "1st sent" => 1,
                    "2nd sent" => 2,
                    _ => -1
                };

                callback.SendToCaller("1st sent", true);
                callback.SendToOthers("branched", true);

            };

            _handler2 = new(port2);
            _handler2.MessageRecieved += (_, callback) => {

                Console.WriteLine($"handler 2 recieved: { callback.Data }");
                if(callback.Data == "1st sent") _cycled = true;

            };

            _handler3 = new(port3);
            _handler3.MessageRecieved += (_, callback) => {

                Console.WriteLine($"handler 3 recieved: { callback.Data }");
                if(callback.Data == "branched") _branched = true;

            };
        }

        [SetUp]
        public void Setup()
        {
            _transmitted = false;
            _cycled = false;
            _branched = false;
        }

        [Test]
        public void TestDatagramPush()
        {
            _handler2.SendMessage("1st sent", true, _endPoint1);

            Thread.Sleep(100);

            Assert.AreEqual(true, _transmitted);
        }

        [Test]
        public void TestDatagramUnreliableAndReliable()
        {
            _handler1.IsListening = false;

            for(int i = 0; i < 100; ++i)
                _handler2.SendMessage("1st sent", false, _endPoint1);
            
            Thread.Sleep(1100);

            Assert.IsFalse(_transmitted);

            _handler2.SendMessage("1st sent", true, _endPoint1);

            Thread.Sleep(1000);

            _handler1.IsListening = true;
            Thread.Sleep(1000);

            Assert.IsTrue(_transmitted);
        }

        [Test]
        public void TestDatagramCycle()
        {
            _handler2.SendMessage("1st sent", true, _endPoint1);
            Thread.Sleep(1000);

            Assert.AreEqual(true, _cycled);
        }

        [Test]
        public void TestCorrectOrder()
        {
            _handler1.IsListening = false;

            _handler2.SendMessage("1st sent", true, _endPoint1);
            Thread.Sleep(500);

            _handler1.IsListening = true;

            _handler2.SendMessage("2nd sent", true, _endPoint1);
            Thread.Sleep(1250);

            Assert.AreEqual(2, _lastRecieved);
        }

        [Test]
        public void TestToOthers()
        {
            _handler3.SendMessage("1st sent", true, _endPoint1);
            _handler2.SendMessage("2nd sent", true, _endPoint1);

            Thread.Sleep(500);

            Assert.AreEqual(true, _branched);
        }
    }
}