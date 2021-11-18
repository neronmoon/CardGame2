using SQLite;

namespace Sources.Database.DataObject {
    public class Character : DataObject<Character>, IDataObject {
        [Unique, NotNull]
        public string Name { get; set; }
        
        public string Sprite { get; set; }
        
        [NotNull]
        public int Health { get; set; }
    }
}
