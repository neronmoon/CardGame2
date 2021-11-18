using System;
using System.Linq;
using Sources.Data.Gameplay;
using SQLite;

namespace Sources.Database.DataObject {
    public enum ItemType {
        Consumable,
        Equippable
    }

    public class Item : DataObject<Item>, IDataObject {
        public string Name { get; set; }
        public string Sprite { get; set; }
        public ItemType Type { get; set; }
        public int Count { get; set; }
        public Strongness Strongness { get; set; }

        [Ignore]
        public ItemEffect[] Effects {
            get => !string.IsNullOrEmpty(EffectsRaw) ? 
                ItemEffect.GetAll().Where(x => EffectsRaw.Split(',').Contains(x.Id.ToString())).ToArray() : 
                Array.Empty<ItemEffect>();
            set => EffectsRaw = string.Join(',', value.Select(x => x.Id.ToString()));
        }

        [Column("Effects")]
        public string EffectsRaw { get; set; }
    }
}
