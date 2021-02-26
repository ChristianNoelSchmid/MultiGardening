namespace Server.Models
{
    public record IdModel : ISerializable
    {
        public uint Id { get; set; }
        public string Serialize() => Id.ToString();
    }
}