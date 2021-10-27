using Leopotam.Ecs;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;

namespace Sources.ECS.Visualization {
    public class ShowDeadScreenSystem : IEcsRunSystem {
        /// <summary>
        /// Shows dead screen
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Player, PlayerDiedEvent> player;

        public void Run() {
            foreach (var idx in player) {
                // TODO: Implement this
            }
        }
    }
}
