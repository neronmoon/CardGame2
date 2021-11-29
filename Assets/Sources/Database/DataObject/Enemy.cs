using System;
using System.Linq;
using Sources.Data.Gameplay;
using SQLite;

namespace Sources.Database.DataObject {
    public class Enemy : DataObject<Enemy>, IDataObject, ICanIncreaseValues {
        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public int Health { get; set; }

        [NotNull]
        public Strongness Strongness { get; set; }

        public string Sprite { get; set; }

        public bool IsAggressive { get; set; }
        
        public void IncreaseValues(float multiplier) {
            Health = (int)(Health * multiplier);
        }
    }
}
