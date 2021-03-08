using System;

namespace Server.State
{
    /// <summary>
    /// Record for a particular Critter's data on the Server
    /// </summary>
    /// <param name="Id">The Critter's unique Id</param>
    /// <param name="Type">The type of Critter</param>
    /// <param name="UpdateTime">The time at which the Critter should be moved (by Server)</param>
    /// <returns></returns>
    public record CritterData(int Id, int Type, DateTime UpdateTime);
}