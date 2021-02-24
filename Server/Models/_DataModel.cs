using System;

namespace Server.Models
{
    /* Serves as a wrapper for JSON objects being sent to and from server.  */
    public record DataModel
    {
        public string Secret { get; init; }
        public int CallerId { get; init; }
    }
    public record DataModel<T>
    {
        public string Secret { get; init; }
        public int CallerId { get; init; }
        public T Value { get; init; }
    }
}