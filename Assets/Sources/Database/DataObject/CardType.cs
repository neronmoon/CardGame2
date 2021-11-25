using SQLite;

namespace Sources.Database.DataObject {
    public class CardType : DataObject<CardType>, IDataObject {
        public const string Enemy = "Enemy";
        public const string Chest = "Chest";
        public const string Item = "Item";
        public const string NPC = "NPC";
        public const string Level = "Level";
        public const string Character = "Character";

        [Unique, NotNull]
        public string Value { get; set; }

        public bool IsA(string type) {
            return Value == type;
        }
    }
}
