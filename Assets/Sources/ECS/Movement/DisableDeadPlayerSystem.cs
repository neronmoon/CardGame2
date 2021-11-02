using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;

namespace Sources.ECS.Movement {
    public class DisableDeadPlayerSystem : IEcsRunSystem {
        /// <summary>
        /// This system disables player if he is dead
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Player, Dead> cards;
        private RuntimeData runtimeData;

        public void Run() {
            foreach (int idx in cards) {
                EcsEntity entity = cards.GetEntity(idx);
                runtimeData.PlayerIsDead = true;
                entity.Del<Draggable>();
            }
        }
    }
}
