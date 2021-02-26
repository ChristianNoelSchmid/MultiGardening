namespace Server.Models
{
    public class PlantPlacement : ISerializable
    {
        public GridPosition GridStart { get; set; }
        public GridPosition GridEnd { get; set; }
        public uint PlantType { get; set; }

        public string Serialize() =>
            $"{GridStart.Serialize()}#{GridEnd.Serialize()}#{PlantType}";
    }
}