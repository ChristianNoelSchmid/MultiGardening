using System;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    public class PlayerJoined : NetworkEvent 
    { 
        public GridPosition Position { get; set; }

        public PlayerJoined() => Position = null;
        public PlayerJoined(string value)
        {
            string [] args = value.Split('#');
            Position = new GridPosition
            {
                X = int.Parse(args[0]), 
                Y = int.Parse(args[1])
            };
        }

        public string CreateString() => $"PlayerJoined::{Position.Serialize()}";
    }
}