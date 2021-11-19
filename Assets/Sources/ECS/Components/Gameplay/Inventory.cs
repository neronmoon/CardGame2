using System.Collections.Generic;
using System.Linq;
using Sources.Database.DataObject;
using UnityEngine;

namespace Sources.ECS.Components.Gameplay {
    public struct Inventory {
        public Dictionary<Item, int> Items;

        private static List<Item> removals = new(5);

        public Inventory(Dictionary<Item, int> items) {
            Items = items;
        }

        public bool Has<T>() where T : Item {
            return Items.Any(pair => pair.Key is T);
        }

        public int Count<T>() where T : Item {
            int count = 0;
            if (Has<T>()) {
                count += Items.Where(pair => pair.Key is T).Sum(pair => pair.Value);
            }

            return count;
        }

        public T TakeOne<T>() where T : Item {
            if (!Has<T>()) {
                Debug.LogWarning("Tried to take item, that not in inventory!");
            }

            Item takenItemData = null;
            foreach ((Item key, int count) in Items) {
                if (key is T) {
                    takenItemData = key;
                    Items[key]--;
                    break;
                }
            }

            cleanupItems();

            return (T)takenItemData;
        }

        public Item TakeOneWithEffect(string effectName) {
            Item item = Items.First(x => x.Key.Effects.Count(e => e.Name == effectName) > 0 && x.Value > 0).Key;

            Items[item]--;
            cleanupItems();

            return item;
        }

        public bool Has(Item itemData) {
            return Items.ContainsKey(itemData);
        }

        public bool HasWithEffect(string name) {
            return Items.Count(x => x.Key.Effects.Count(e => e.Name == name) > 0 && x.Value > 0) > 0;
        }

        public Inventory Add(Item type, int count = 1) {
            if (Has(type)) {
                Items[type] += count;
            } else {
                Items.Add(type, count);
            }

            return this;
        }

        private void cleanupItems() {
            foreach (KeyValuePair<Item, int> pair in Items.Where(pair => pair.Value <= 0)) {
                removals.Add(pair.Key);
            }

            foreach (Item item in removals) {
                Items.Remove(item);
            }

            removals.Clear();
        }
    }
}
