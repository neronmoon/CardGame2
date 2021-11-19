using JetBrains.Annotations;
using Sources.Data.Gameplay;

namespace Sources.Database.DataObject {
    public class Enemy : DataObject<Enemy>, IDataObject, ICanIncreaseValues {
        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public int Health { get; set; }

        [NotNull]
        public Strongness Strongness { get; set; }

        public string Sprite { get; set; }

        public void IncreaseValues(float multiplier) {
            Health = (int)(Health * multiplier);
        }
    }
}
