using Sources.Data.Gameplay;

namespace Sources.Database.DataObject {
    public class Chest : CardsContainer<Chest> {
        public Strongness Strongness { get; set; }
        public string SpritePath { get; set; }
    }
}
