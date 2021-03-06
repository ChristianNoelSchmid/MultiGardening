using System;

namespace Server.Models {
    /// <summary>
    /// All plant placement state, regarding when and where a Client
    /// places a plant. Forwarded to other clients, and used to
    /// check placement in the future, and placement of critters.
    /// </summary>
    public record PlantPlacement(

        GridPosition Position, int PlantType, 

        // Represents the total seconds, since the server started,
        // to the point when the plant is fully grown.
        DateTime TimeToComplete
    
    ) : ISerializable {
        public string Serialize() =>
            $"{Position.Serialize()}#{PlantType}#{TimeToComplete.ToString("yyyy-MM-dd HH:mm:ss")}";
    }
}