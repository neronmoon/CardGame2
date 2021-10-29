using Leopotam.Ecs;
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

        public void Run() {
            foreach (int idx in cards) {
                if (cards.Get1(idx).Amount <= 0) {
                    EcsEntity entity = cards.GetEntity(idx);
                    entity.Replace(new Dead());
                    if (entity.Has<Player>()) {
                        entity.Replace(new PlayerDiedEvent());
                    }
                }
            }
        }
    }
}
