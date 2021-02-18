using System;

namespace GameServer.Networking
{
    public record Datagram<T>
    {
        public Guid CallerId { get; init; }
        public T Value { get; init; }
    }
}