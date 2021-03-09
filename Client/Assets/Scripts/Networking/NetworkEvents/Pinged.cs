using System;
using Server.Models;

namespace Server.Networking.NetworkEvents 
{
    /// <summary>
    /// NetworkEvent representing a Client that
    /// has pinged the Server
    /// </summary>
    public class Pinged : NetworkEvent 
    {
        public string CreateString() => "Pinged";
    }
}