using System.Collections.Generic;
using System.Linq;
using Leopotam.Ecs;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.GameplayActions.Components;
using Sources.LevelGeneration;
using UnityEngine;
using Random = System.Random;

namespace Sources.ECS.GameplayActions {
    public class EnemyDropLootSystem : IEcsRunSystem {
        /// <summary>
        /// When enemy dies without change their level position -- it should be replaced with some item
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Enemy, Dead, LevelPosition>.Exclude<Discarded> filter;

        private Random random = new();
        private CardEntityGenerator cardEntityGenerator;

        public void Run() {
            foreach (int idx in filter) {
                object item = Choose(filter.Get1(idx).Data.DropItems());
                filter.GetEntity(idx).Replace(new Discarded());
                LevelPosition pos = filter.Get3(idx);
                cardEntityGenerator.CreateCardEntity(item, pos.X, pos.Y);
            }
        }

        private T Choose<T>(IEnumerable<KeyValuePair<T, int>> items) { // TODO: remove copypaste
            List<KeyValuePair<T, int>> sortedItems = items.ToList();
            sortedItems.Sort((x, xx) => x.Value - xx.Value);
            if (sortedItems.Count < 1) {
                Debug.LogError("Chances of " + typeof(T) + " is empty!");
                return default;
            }

            int max = sortedItems.Sum(x => x.Value);
            int prev = 0;
            int c = random.Next(0, max);
            foreach (KeyValuePair<T, int> item in sortedItems) {
                if (c >= prev && c <= item.Value + prev) {
                    return item.Key;
                }

                prev += item.Value;
            }

            Debug.LogError("Chances of " + typeof(T) + " is empty!");
            return default;
        }
    }
}
