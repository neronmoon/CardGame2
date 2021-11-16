using Sources.Data.Gameplay;

namespace Sources.Database.DataObject {
    public class Enemy : DataObject<Enemy>, IDataObject {
        public string Name {get; set;}
        public int Health {get; set;}
        public Strongness Strongness {get; set;}

        public string SpritePath {get; set;}
    }
}
