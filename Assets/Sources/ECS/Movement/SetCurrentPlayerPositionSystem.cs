using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;

namespace Sources.ECS.Movement {
    public class SetCurrentPlayerPositionSystem : IEcsRunSystem {
        /// <summary>
        /// This system handles RuntimeData's Player position property
        /// </summary>
        private EcsWorld world;

        private EcsFilter<PlayableCard, LevelPosition, Player> player;
        private RuntimeData runtimeData;

        public void Run() {
            foreach (int idx in player) {
                // runtimeData.PlayerPosition = player.Get2(idx);
            }
        }
    }
}

