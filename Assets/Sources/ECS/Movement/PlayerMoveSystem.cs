using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;

namespace Sources.ECS.Movement {
    public class PlayerMoveSystem : IEcsRunSystem {
        /// <summary>
        /// This system reacts to drop event and sets player level position
        /// </summary>
        private EcsWorld world;

        private EcsFilter<DroppedEvent> dropped;
        private EcsFilter<PlayableCard, Player, LevelPosition> player;
        private RuntimeData runtimeData;

        public void Run() {
            if (dropped.IsEmpty()) return;

            foreach (int idx in dropped) {
                EcsEntity dropzone = dropped.Get1(idx).DropZone;
                if (!dropzone.Has<LevelPosition>()) return;
                foreach (var playerIdx in player) {
                    EcsEntity entity = player.GetEntity(playerIdx);
                    entity.Replace(dropzone.Get<LevelPosition>());
                    runtimeData.GarbageEntity.Replace(new PlayerMovedEvent { Player = entity, Card = dropzone });
                }
            }
        }
    }
}
