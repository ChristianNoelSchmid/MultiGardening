using System;
using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents
{
    /// <summary>
    /// NetworkEvent representing a new Client that
    /// has joined the game.
    /// </summary>
    public record PlayerJoined : NetworkEvent 
    { 
        public GridPosition Position { get; init; }

        public PlayerJoined() => Position = null;
        public PlayerJoined(string value) {

            string [] args = value.Split('#');

            Position = new GridPosition(
                int.Parse(args[0]), 
                int.Parse(args[1])
            );
        }

        public string CreateString() => $"PlayerJoined::{Position.Serialize()}";
    }
}