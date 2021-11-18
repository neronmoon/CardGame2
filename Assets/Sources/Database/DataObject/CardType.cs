using SQLite;

namespace Sources.Database.DataObject {
    public class CardType : DataObject<CardType>, IDataObject {
        [Unique, NotNull]
        public string Value { get; set; }
    }
}
