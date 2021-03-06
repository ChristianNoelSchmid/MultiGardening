using System;

namespace Server.State
{
    public record PlantData
    {
        public int Type { get; init; }
        public DateTime FullyGrownTime { get; init; }
    }
}