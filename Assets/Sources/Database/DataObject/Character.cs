namespace Sources.Database.DataObject {
    public class Character : DataObject<Character>, IDataObject {
        public string Name { get; set; }
        public string SpritePath { get; set; }
        public int Health { get; set; }
    }
}
