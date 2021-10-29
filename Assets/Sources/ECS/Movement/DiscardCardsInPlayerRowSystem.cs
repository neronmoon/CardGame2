using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;

namespace Sources.ECS.Movement {
    public class DiscardCardsInPlayerRowSystem : IEcsRunSystem {
        /// <summary>
        /// This system removes cards, that cannot be played after player move
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;
        private EcsFilter<CompleteStep> stepCompleted;
        private EcsFilter<PlayableCard, LevelPosition>.Exclude<Player> cards;

        public void Run() {
            if (stepCompleted.IsEmpty()) return;
            foreach (int idx in cards) {
                LevelPosition pos = cards.Get2(idx);
                if (pos.Y <= runtimeData.PlayerPosition.Y) {
                    cards.GetEntity(idx).Replace(new Discarded());
                }
            }
        }
    }
}
