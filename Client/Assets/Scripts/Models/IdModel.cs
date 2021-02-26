namespace Server.Models
{
    public class IdModel : ISerializable
    {
        public uint Id { get; set; }
        public string Serialize() => Id.ToString();
    }
}