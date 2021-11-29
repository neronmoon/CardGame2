using System.Collections.Generic;
using System.Linq;
using Sources.Database.DataObject;

namespace Sources.ECS.Components.Gameplay {
    public struct Inventory {
        public List<Item> Items;
        private List<Item> removals;

        public Inventory AddItem(Item item, int amount = 1) {
            for (int i = 0; i < amount; i++) {
                Items.Add(item);
            }

            return this;
        }

        public bool HasItem(string name) {
            return Items.Count(x => x.Name == name) == 0;
        }

        public bool HasItemWithEffect(ItemEffectType name) {
            return Items.SelectMany(item => item.Effects).Any(effect => effect.Name == name);
        }

        public List<Item> TakeItem(string name, int amount = 1) {
            List<Item> taken = new(amount);
            foreach (Item item in Items.Where(x => x.Name == name).ToList().TakeWhile(item => taken.Count <= amount)) {
                taken.Add(item);
                removeItem(item);
            }

            cleanupInventory();

            return taken;
        }

        public Item TakeFirstItemWithEffect(ItemEffectType type) {
            return TakeItemWithEffect(type).First();
        }

        public List<Item> TakeItemWithEffect(ItemEffectType type, int amount = 1) {
            List<Item> taken = new(amount);
            foreach (Item item in Items.TakeWhile(_ => taken.Count <= amount).Where(item => item.Effects.Any(x => x.Name == type))) {
                taken.Add(item);
                removeItem(item);
            }

            cleanupInventory();

            return taken;
        }

        private void removeItem(Item item) {
            removals ??= new List<Item>();
            removals.Add(item);
        }

        private void cleanupInventory() {
            foreach (var itemToRemove in removals) {
                Items.Remove(itemToRemove);
            }
        }
    }
}
