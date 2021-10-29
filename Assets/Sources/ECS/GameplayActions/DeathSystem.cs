using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;

namespace Sources.ECS.GameplayActions {
    public class DeathSystem : IEcsRunSystem {
        /// <summary>
        /// This system adds Dead component to card, if it's health is <= 0 and fires event
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Health> cards;
        private RuntimeData runtimeData;

        public void Run() {
            foreach (int idx in cards) {
                if (cards.Get1(idx).Amount <= 0) {
                    EcsEntity entity = cards.GetEntity(idx);
                    entity.Replace(new Dead());
                    if (entity.Has<Player>()) {
                        runtimeData.PlayerIsDead = true;
                        entity.Del<Draggable>();
                        entity.Replace(new PlayerDiedEvent());
                    }
                }
            }
        }
    }
}
