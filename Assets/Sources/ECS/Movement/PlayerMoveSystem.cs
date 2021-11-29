using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;

namespace Sources.ECS.Movement {
    public class PlayerMoveSystem : IEcsRunSystem {
        /// <summary>
        /// This system reacts to drop event and sets player level position
        /// </summary>
        private EcsWorld world;

        private EcsFilter<DroppedEvent> dropped;
        private EcsFilter<PlayableCard, LevelPosition, Player> player;
        private RuntimeData runtimeData;

        public void Run() {
            if (dropped.IsEmpty()) return;

            foreach (int idx in dropped) {
                EcsEntity dropzone = dropped.Get1(idx).DropZone;
                if (!dropzone.Has<LevelPosition>()) return;
                foreach (int playerIdx in player) {
                    EcsEntity playerEnt = player.GetEntity(playerIdx);
                    playerEnt.Replace(dropzone.Get<LevelPosition>());
                    playerEnt.Replace(new PlayerMovedEvent { Target = dropzone });
                }
            }
        }
    }
}
