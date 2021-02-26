using System;

namespace Server.Models
{
    /* Serves as a wrapper for JSON objects being sent to and from server.  */
    public record DataModel : ISerializable
    {
        public string Secret { get; init; }
        public int CallerId { get; init; }

        public string Serialize() =>
            string.Join('#', CallerId, Secret);
    }
    public record DataModel<T> : ISerializable
        where T : ISerializable
    {
        public string Secret { get; init; }
        public int CallerId { get; init; }
        public T Value { get; init; }

        public string Serialize() =>
            string.Join('#', CallerId, Secret) + $"#{Value.Serialize()}";
    }
}