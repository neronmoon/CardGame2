using SQLite;

namespace Sources.Database.DataObject {
    public class Level : CardsContainer<Level> {
        [NotNull]
        public float Difficulty { get; set; }

        public string Sprite { get; set; }
    }
}
