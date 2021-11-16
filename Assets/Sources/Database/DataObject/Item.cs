using System;
using System.Linq;
using Sources.Data.Gameplay;
using SQLite;

namespace Sources.Database.DataObject {

    public enum ItemType {
        Consumable, Equippable
    }
    
    public class Item : DataObject<Item>, IDataObject {
        public string Name;
        public string SpritePath { get; set; }
        public ItemType Type;
        public Strongness Strongness { get; set; }
        
        [Ignore]
        public ItemEffect[] Effects {
            get => ItemEffect.GetAll().Where(x => EffectsRaw.Split(',').Contains(x.Id.ToString())).ToArray();
            set => EffectsRaw = string.Join(',', value.Select(x => x.Id.ToString()));
        }

        [Column("Effects")]
        public string EffectsRaw { get; set; }

    }
}
