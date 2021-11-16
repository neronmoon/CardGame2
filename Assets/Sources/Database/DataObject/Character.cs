namespace Sources.Database.DataObject {
    public class Character : DataObject<Character>, IDataObject {
        public string Name { get; set; }
        public string Sprite { get; set; }
        public int Health { get; set; }
    }
}
