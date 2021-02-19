using System;

namespace Server.Networking
{
    public record DataModel
    {
        public string Secret { get; init; }
        public Guid CallerId { get; init; }
    }
    public record DataModel<T>
    {
        public string Secret { get; init; }
        public Guid CallerId { get; init; }
        public T Value { get; init; }
    }
}