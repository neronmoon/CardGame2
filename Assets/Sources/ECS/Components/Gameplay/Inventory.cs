using System.Collections.Generic;
using System.Linq;
using Sources.Data.Gameplay.Items;
using Sources.ECS.GameplayActions.Components;
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

            Item takenItem = null;
            foreach ((Item key, int count) in Items) {
                if (key is T) {
                    takenItem = key;
                    Items[key]--;
                    break;
                }
            }

            cleanupItems();

            return (T)takenItem;
        }

        public bool Has(Item item) {
            return Items.ContainsKey(item);
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
