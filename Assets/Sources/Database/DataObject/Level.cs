namespace Sources.Database.DataObject {
    public class Level : CardsContainer<Level> {
        public float Difficulty { get; set; }
        public string SpritePath { get; set; }
    }
}
