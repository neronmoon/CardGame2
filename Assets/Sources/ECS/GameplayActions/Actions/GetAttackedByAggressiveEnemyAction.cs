using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;
using UnityEngine;

namespace Sources.ECS.GameplayActions.Actions {
    public class GetAttackedByAggressiveEnemyAction : IGameplayAction {
        private EcsWorld world;
        private EcsFilter<ECS.Components.Gameplay.CardTypes.Enemy, Health, LevelPosition, Spawned>.Exclude<Dead> filter;
        private RuntimeData runtimeData;

        public GetAttackedByAggressiveEnemyAction(
            EcsFilter<ECS.Components.Gameplay.CardTypes.Enemy, Health, LevelPosition, Spawned>.Exclude<Dead> filter,
            RuntimeData runtimeData
        ) {
            this.filter = filter;
            this.runtimeData = runtimeData;
        }

        public bool ShouldAct(EcsEntity target) {
            if (target.Has<PreHit>()) return false;
            return GetAggressiveEnemy(target) != default;
        }

        public object[] Act(EcsEntity target) {
            EcsEntity enemy = GetAggressiveEnemy(target);
            if (enemy == default) {
                return new object[] { };
            }

            return new object[] {
                new PreHit { Hit = new Hit { Source = enemy, Amount = enemy.Get<Health>().Value, ByPlayer = false } }
            };
        }

        private EcsEntity GetAggressiveEnemy(EcsEntity target) {
            int nextRowY = target.Get<LevelPosition>().Y + 1;
            if (runtimeData.LevelLayout.Length <= nextRowY || nextRowY == 1) {
                return default; // end of level or first row
            }

            foreach (int idx in filter) {
                LevelPosition enemyPos = filter.Get3(idx);
                if (enemyPos.Y == nextRowY && enemyPos.X == target.Get<LevelPosition>().X) {
                    return filter.GetEntity(idx);
                }
            }

            return default;
        }
    }
}
