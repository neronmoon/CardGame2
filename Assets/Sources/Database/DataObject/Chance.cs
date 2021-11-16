using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Sources.Database.DataObject {
    public class Chance : DataObject<Chance>, IDataObject {
        public string ContainerType { get; set; }
        public int ContainerId { get; set; }
        public string ItemType { get; set; }
        public int ItemId { get; set; }
        public int Probability { get; set; }
        
        public static KeyValuePair<T, int>[] ChancesFor<T>(IDataObject container) where T : new() {
            Chance[] chances = Find(chance =>
                chance.ContainerType == container.GetType().Name &&
                chance.ContainerId == container.GetId() &&
                chance.ItemType == typeof(T).Name
            );
            KeyValuePair<T, int>[] result = new KeyValuePair<T, int>[chances.Length];
            int i = 0;
            foreach (Chance chance in chances) {
                List<T> item = GetConnection().Query<T>($"Select * from {typeof(T).Name} where Id = {chance.ItemId} limit 1;");
                result[i] = new KeyValuePair<T, int>(item.First(), chance.Probability);
                i++;
            }

            return result;
        }

        public static Chance Make(IDataObject container, IDataObject item, int probability) {
            return new Chance {
                ContainerType = container.GetType().Name,
                ContainerId = container.GetId(),
                ItemType = item.GetType().Name,
                ItemId = item.GetId(),
                Probability = probability
            };
        }
    }
}
