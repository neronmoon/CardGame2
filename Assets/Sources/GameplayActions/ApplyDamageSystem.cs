using Leopotam.Ecs;
using Sources.ECS.Components.Gameplay;
using Sources.GameplayActions.Components;
using UnityEngine;

namespace Sources.GameplayActions {
    public class ApplyDamageSystem : IEcsRunSystem {
        /// <summary>
        /// This system applies damage from hit
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Health, Hit> hits;

        public void Run() {
            foreach (var idx in hits) {
                EcsEntity entity = hits.GetEntity(idx);
                int dmg = hits.Get2(idx).Amount;
                int health = hits.Get1(idx).Amount;
                entity.Replace(new Health { Amount = health - dmg });
                Debug.Log($"Hit by {dmg}!");
            }
        }
    }
}