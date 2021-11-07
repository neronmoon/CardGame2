using System.Collections.Generic;
using System.Linq;
using Sources.Data.Gameplay.Items;
using Sources.ECS.GameplayActions.Components;
using UnityEngine;

namespace Sources.ECS.Components.Gameplay {
    public struct Inventory {
        public Dictionary<ItemData, int> Items;

        private static List<ItemData> removals = new(5);

        public Inventory(Dictionary<ItemData, int> items) {
            Items = items;
        }

        public bool Has<T>() where T : ItemData {
            return Items.Any(pair => pair.Key is T);
        }

        public int Count<T>() where T : ItemData {
            int count = 0;
            if (Has<T>()) {
                count += Items.Where(pair => pair.Key is T).Sum(pair => pair.Value);
            }

            return count;
        }

        public T TakeOne<T>() where T : ItemData {
            if (!Has<T>()) {
                Debug.LogWarning("Tried to take item, that not in inventory!");
            }

            ItemData takenItemData = null;
            foreach ((ItemData key, int count) in Items) {
                if (key is T) {
                    takenItemData = key;
                    Items[key]--;
                    break;
                }
            }

            cleanupItems();

            return (T)takenItemData;
        }

        public bool Has(ItemData itemData) {
            return Items.ContainsKey(itemData);
        }

        public Inventory Add(ItemData type, int count = 1) {
            if (Has(type)) {
                Items[type] += count;
            } else {
                Items.Add(type, count);
            }

            return this;
        }

        private void cleanupItems() {
            foreach (KeyValuePair<ItemData, int> pair in Items.Where(pair => pair.Value <= 0)) {
                removals.Add(pair.Key);
            }

            foreach (ItemData item in removals) {
                Items.Remove(item);
            }

            removals.Clear();
        }
    }
}
