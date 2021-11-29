using System;
using Leopotam.Ecs;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;

namespace Sources.ECS.GameplayActions {
    public class ApplyDamageSystem : IEcsRunSystem {
        /// <summary>
        /// This system applies damage from hit
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Health, Hit> hits;

        public void Run() {
            foreach (int idx in hits) {
                EcsEntity entity = hits.GetEntity(idx);
                Hit dmg = hits.Get2(idx);
                int health = hits.Get1(idx).Value;
                entity.Replace(new Health { Value = Math.Max(0, health - dmg.Amount) });
                if (dmg.Source != default) {
                    dmg.Source.Replace(new Dead());
                }
            }
        }
    }
}
