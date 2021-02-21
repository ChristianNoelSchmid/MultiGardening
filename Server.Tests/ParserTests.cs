using System;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using Server.Models;
using Server.Networking;
using Server.Networking.NetworkEvents;

namespace Server.Tests
{
    public class ParserTests
    {
        private readonly Guid guid 
            = Guid.Parse("12345678-9101-1121-3141-516171819202");
        private readonly NetworkEventHandler = new NetworkEventHandler();

        [Test]
        public void TestPlayerJoinedParse()
        {
            var playerJoined = new 
            {
                Position = Tuple.Create(1.0f, 2.0f),
                IsFlipped = true
            };
            Assert.AreEqual(
                new PlayerJoined
                {
                    ActorMovement = new ActorMovement
                    {
                        Position = Tuple.Create(1.0f, 2.0f),
                        IsFlipped = true
                    }
                },
                NetworkEventHandler.ParseEvent($"PlayerJoined::{ JsonSerializer.Serialize(playerJoined) }")
            );
        }

        [Test]
        public void TestPlayerLeftParse()
        {
            var playerLeft = new 
            {
                CallerId = guid,
                Secret = "Secret"
            };

            Assert.AreEqual(
                new PlayerLeft
                {
                    CallerInfo = new DataModel
                    {
                        CallerId = guid,
                        Secret = "Secret"
                    }
                },
                NetworkEventHandler.ParseEvent($"PlayerLeft::{ JsonSerializer.Serialize(playerLeft)} ")
            );
        }
    
        [Test]
        public void TestTilledParse()
        {
            var tilled = new    
            {
                CallerId = guid,
                Secret = "Secret",
                Value = new 
                {
                    X = 10,
                    Y = 10
                }
            };

            Assert.AreEqual(
                new Tilled
                {
                    Position = new DataModel<GridPosition>
                    {
                        CallerId = guid,
                        Secret = "Secret",
                        Value = new GridPosition
                        {
                            X = 10,
                            Y = 10
                        }
                    }
                },
                NetworkEventHandler.ParseEvent($"Tilled::{ JsonSerializer.Serialize(tilled) }")
            );
        } 
    
        [Test] 
        public void TestPlantedParse()
        {
            var planted = new 
            {
                CallerId = guid,
                Secret = "Secret",
                Value = new
                {
                    GridStart = new { X = 10, Y = 10 },
                    GridEnd = new { X = 11, Y = 11 },
                    PlantType = 2
                }
            };

            Assert.AreEqual(
                new Planted
                {
                    Placement = new DataModel<PlantPlacement>
                    {
                        CallerId = guid,
                        Secret = "Secret",
                        Value = new PlantPlacement
                        {
                            GridStart = new GridPosition { X = 10, Y = 10 },
                            GridEnd = new GridPosition { X = 11, Y = 11 },
                            PlantType = 2
                        }
                    }
                },
                NetworkEventHandler.ParseEvent($"Planted::{ JsonSerializer.Serialize(planted) }")
            );
        }
    
        [Test]
        public void TestDestroyedParse()
        {
            var destroyed = new 
            {
                CallerId = guid,
                Secret = "Secret",
                Value = 3
            };

            Assert.AreEqual(
                new Destroyed
                {
                    PlantIndex = new DataModel<uint>
                    {
                        CallerId = guid,
                        Secret = "Secret",
                        Value = 3
                    }
                },
                NetworkEventHandler.ParseEvent($"Destroyed::{ JsonSerializer.Serialize(destroyed) }")
            );
        } 

        [Test]
        public void TestPingedParse()
        {
            var pinged = new 
            {
                CallerId = guid,
                Secret = "Secret"
            };

            Assert.AreEqual(
                new Pinged
                {
                    CallerInfo = new DataModel
                    {
                        CallerId = guid,
                        Secret = "Secret"
                    }
                },
                NetworkEventHandler.ParseEvent($"Pinged::{ JsonSerializer.Serialize(pinged) }")
            );
        }
    }
}