using Sources.Data.Gameplay;

namespace Sources.Database.DataObject {
    public class Chest : CardsContainer<Chest> {
        public Strongness Strongness { get; set; }
        public string Sprite { get; set; }
        
        public string Description { get; set; }
    }
}
