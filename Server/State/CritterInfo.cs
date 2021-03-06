using System.Collections.Immutable;

namespace Server.State
{
    public record CritterInfo
    {
        public uint CritterType { get; init; }
        public ImmutableList<int> PlantTypeAttractions { get; init; }
        public float SecondsToUpdate { get; init; }
    }
}