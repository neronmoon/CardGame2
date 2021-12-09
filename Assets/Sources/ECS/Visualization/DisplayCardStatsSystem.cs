using Leopotam.Ecs;
using Sources.ECS.Components;
using Sources.Unity;

namespace Sources.ECS.Visualization {
    public class DisplayCardStatsSystem : IEcsRunSystem {
        /// <summary>
        /// This system is reflecting ecs entity data into card view
        /// </summary>
        private EcsWorld world;

        private EcsFilter<PlayableCard, VisualObject> cards;

        public void Run() {
            foreach (int idx in cards) {
                CardView view = cards.Get2(idx).Object.GetComponent<CardView>();
                view.FillStats(cards.GetEntity(idx));
            }
        }
    }
}
