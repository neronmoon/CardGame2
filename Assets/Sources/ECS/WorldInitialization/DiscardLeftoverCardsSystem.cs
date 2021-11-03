using Leopotam.Ecs;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;

namespace Sources.ECS.WorldInitialization {
    public class DiscardLeftoverCardsSystem : IEcsRunSystem {
        /// <summary>
        /// This system discards cards, that left from previous level
        /// </summary>
        private EcsWorld world;

        private EcsFilter<StartLevelEvent> started;

        private EcsFilter<PlayableCard>.Exclude<Player> cards;

        public void Run() {
            if (started.IsEmpty()) return;
            foreach (var idx in cards) {
                cards.GetEntity(idx)
                     .Replace(new Discarded())
                     .Replace(new Leftover());
            }
        }
    }
}
