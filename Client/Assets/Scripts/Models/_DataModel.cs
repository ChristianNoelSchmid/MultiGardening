using System;

namespace Server.Models
{
    /* Serves as a wrapper for JSON objects being sent to and from server.  */
    public class DataModel : ISerializable
    {
        public string Secret { get; set; }
        public int CallerId { get; set; }

        public string Serialize() =>
            string.Join("#", CallerId, Secret);
    }

    public class DataModel<T> : ISerializable
        where T : ISerializable
    {
        public string Secret { get; set; }
        public int CallerId { get; set; }
        public T Value { get; set; }

        public string Serialize() =>
            string.Join("#", CallerId, Secret) + $"#{Value.Serialize()}";
    }
}