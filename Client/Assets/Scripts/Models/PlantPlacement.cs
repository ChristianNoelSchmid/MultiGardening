namespace Server.Models
{
    public class PlantPlacement
    {
        public GridPosition GridStart { get; set; }
        public GridPosition GridEnd { get; set; }
        public uint PlantType { get; set; }
    }
}