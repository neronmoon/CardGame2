using Sources.Data.Gameplay;

namespace Sources.Database.DataObject {
    public class ItemEffect :DataObject<ItemEffect>, IDataObject, ICanIncreaseValues {
        public string Name { get; set; }
        public float Value { get; set; }
        public Strongness Strongness { get; set; }
        
        public void IncreaseValues(float multiplier) {
            Value *= multiplier;
        }
    }
}
