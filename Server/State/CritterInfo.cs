using System.Collections.Immutable;

namespace Server.State
{
    /// <summary>
    /// Represents static data referencing various helpful
    /// information pertaining to Critters for the Server to use.
    /// </summary>
    /// <param name="CritterType">The type of Critter</param>
    /// <param name="PlantTypeAttractions">What type of plants the Critter likes</param>
    /// <param name="SecondsToUpdate">The amount of seconds it should take from the Critter to move</param>
    /// <returns></returns>
    public record CritterInfo (
        uint CritterType, 
        ImmutableList<int> PlantTypeAttractions, 
        float SecondsToUpdate
    );
}