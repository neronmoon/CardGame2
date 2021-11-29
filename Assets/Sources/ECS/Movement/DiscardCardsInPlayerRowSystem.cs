using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Extensions;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
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
        private EcsFilter<PlayableCard, LevelPosition, Player> playerCard;

        public void Run() {
            if (stepCompleted.IsEmpty()) return;

            int playerPosition = playerCard.GetComponentOnFirstOrDefault(new LevelPosition { X = 1, Y = 0 }).Y;

            foreach (int idx in cards) {
                LevelPosition pos = cards.Get2(idx);
                if (pos.Y <= playerPosition) {
                    cards.GetEntity(idx).Replace(new Discarded());
                }
            }
        }
    }
}
