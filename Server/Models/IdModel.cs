namespace Server.Models {
    public record IdModel(int Id) : ISerializable {
        public string Serialize() => Id.ToString();
    }
}