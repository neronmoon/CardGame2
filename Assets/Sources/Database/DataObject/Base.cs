using System.Collections.Generic;
using SQLite;

namespace Sources.Database.DataObject {
    public interface ILevelDefinition {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Length { get; set; }
        public int Width { get; set; }
    }

    public interface ICanIncreaseValues {
        public void IncreaseValues(float multiplier);
    }

    public abstract class CardsContainer<T> : DataObject<T>, ILevelDefinition, IDataObject where T : IDataObject, new() {
        [Unique, NotNull]
        public string Name { get; set; }

        public string Description { get; set; }

        [NotNull]
        public int Length { get; set; }

        [NotNull]
        public int Width { get; set; }

        public KeyValuePair<TItemType, int>[] Chances<TItemType>() where TItemType : new() {
            return Chance.ChancesFor<TItemType>(this);
        }
    }
}
