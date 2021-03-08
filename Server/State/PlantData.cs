using System;

namespace Server.State
{
    /// <summary>
    /// Record for a particular Plant's data on the Server
    /// </summary>
    /// <param name="Type">The Plant's type</param>
    /// <param name="FullyGrownTime">The time at which the Plant will be fully grown</param>
    public record PlantData(
        int Type,
        DateTime FullyGrownTime
    );
}