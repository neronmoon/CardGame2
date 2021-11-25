using System;
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
                Health health = filter.Get1(idx);

                int newHealthValue = health.Value + healAmount;
                if (filter.GetEntity(idx).Has<MaxHealth>()) {
                    newHealthValue = Math.Min(newHealthValue, filter.GetEntity(idx).Get<MaxHealth>().Value);
                }

                filter.GetEntity(idx).Replace(new Health { Value = newHealthValue });
            }
        }
    }
}
