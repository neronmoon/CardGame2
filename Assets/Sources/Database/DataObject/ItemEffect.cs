using Sources.Data.Gameplay;
using SQLite;

namespace Sources.Database.DataObject {
    [StoreAsText]
    public enum ItemEffectType {
        Heal,
        Resurrection,
    }

    public class ItemEffect : DataObject<ItemEffect>, IDataObject, ICanIncreaseValues {
        [NotNull]
        public ItemEffectType Name { get; set; }

        [NotNull]
        public float Value { get; set; }

        [NotNull]
        public Strongness Strongness { get; set; }

        public void IncreaseValues(float multiplier) {
            Value *= multiplier;
        }
    }
}
