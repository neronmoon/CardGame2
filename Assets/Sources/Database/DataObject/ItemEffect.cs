using Sources.Data.Gameplay;
using SQLite;

namespace Sources.Database.DataObject {
    public class ItemEffect : DataObject<ItemEffect>, IDataObject, ICanIncreaseValues {
        [NotNull]
        public string Name { get; set; }

        [NotNull]
        public float Value { get; set; }

        [NotNull]
        public Strongness Strongness { get; set; }

        public void IncreaseValues(float multiplier) {
            Value *= multiplier;
        }
    }
}
