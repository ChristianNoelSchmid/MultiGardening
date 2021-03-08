using System;

namespace Server.Models
{
    /* Serves as a wrapper for serialized objects being sent to and from server.  */
    public record DataModel (string Secret, int CallerId) : ISerializable {
        public string Serialize() =>
            string.Join('#', CallerId, Secret);
    }
    public record DataModel<T>(string Secret, int CallerId, T Value) : ISerializable
        where T : ISerializable {
        public string Serialize() =>
            string.Join('#', CallerId, Secret) + $"#{Value.Serialize()}";
    }
}