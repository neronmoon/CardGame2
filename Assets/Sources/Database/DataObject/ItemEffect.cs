using Sources.Data.Gameplay;

namespace Sources.Database.DataObject {
    public class ItemEffect :DataObject<ItemEffect>, IDataObject {
        public string Name { get; set; }
        public float Value { get; set; }
        public Strongness Strongness { get; set; }
    }
}
