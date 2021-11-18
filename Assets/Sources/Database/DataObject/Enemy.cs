using Sources.Data.Gameplay;

namespace Sources.Database.DataObject {
    public class Enemy : DataObject<Enemy>, IDataObject, ICanIncreaseValues {
        public string Name {get; set;}
        public int Health {get; set;}
        public Strongness Strongness {get; set;}

        public string Sprite {get; set;}
        
        public void IncreaseValues(float multiplier) {
            Health = (int)(Health * multiplier);
        }
    }
}
