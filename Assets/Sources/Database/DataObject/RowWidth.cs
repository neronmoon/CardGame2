using SQLite;

namespace Sources.Database.DataObject {
    public class RowWidth : DataObject<RowWidth>, IDataObject {
        [Unique, NotNull]
        public int Value { get; set; }
    }
}
