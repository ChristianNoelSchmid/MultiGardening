namespace Server.State
{
    /// <summary>
    /// Represents static data referencing various helpful
    /// information pertaining to Plants for the Server to use.
    /// </summary>
    /// <param name="PlantType">The Plant's type</param>
    /// <param name="SecondsToGrow">The amount of seconds it should take for the Plant to fully grow</param>
    /// <returns></returns>
    public record PlantInfo (
        uint PlantType,
        float SecondsToGrow
    );
}