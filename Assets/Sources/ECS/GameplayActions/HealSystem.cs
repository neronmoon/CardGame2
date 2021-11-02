using Leopotam.Ecs;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;

namespace Sources.ECS.GameplayActions {
    public class HealSystem : IEcsRunSystem {
        /// <summary>
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Health, Heal> filter;

        public void Run() {
            foreach (int idx in filter) {
                int healAmount = filter.Get2(idx).Amount;
                int health = filter.Get1(idx).Amount;

                filter.GetEntity(idx).Replace(new Health { Amount = health + healAmount });
            }
        }
    }
}

