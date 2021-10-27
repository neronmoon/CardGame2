using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;

namespace Sources.ECS.Movement {
    public class DiscardCardsInPlayerRowSystem : IEcsRunSystem {
        /// <summary>
        /// This system removes cards, that cannot be played after player move
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;
        private EcsFilter<PlayerMovedEvent> moved;
        private EcsFilter<PlayableCard, LevelPosition>.Exclude<Player> cards;

        public void Run() {
            // if (moved.IsEmpty()) return;
            foreach (var idx in cards) {
                LevelPosition pos = cards.Get2(idx);
                if (pos.Y <= runtimeData.PlayerPosition.Y) {
                    cards.GetEntity(idx).Replace(new Discarded());
                }
            }
        }
    }
}
