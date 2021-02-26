using System;
using System.Linq;

namespace Server.Models
{
    public interface ISerializable 
    {
        string Serialize();
    }
}