using System;
using System.Text.Json;
using Server.Models;

namespace Server.Networking.NetworkEvents 
{
    /// <summary>
    /// NetworkEvent representing a Client that
    /// has pinged the Server
    /// </summary>
    public record Pinged : NetworkEvent 
    {
        public string CreateString() => "Pinged";
    }
}