using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;

namespace Sources.ECS.Movement {
    public class SetAvailableDropZones : IEcsRunSystem {
        /// <summary>
        /// This systems adds/removes dropzone component to card entitites to define which card is available to move on
        /// Basicly it defines constraint to player movement
        /// </summary>
        private EcsWorld world;

        private EcsFilter<PlayableCard, LevelPosition, VisualObject>.Exclude<Player> cards;
        private RuntimeData runtimeData;

        public void Run() {
            foreach (var idx in cards) {
                LevelPosition levelPosition = cards.Get2(idx);

                bool availableDropZone = levelPosition.Y == runtimeData.CurrentPlayerPosition + 1;
                EcsEntity entity = cards.GetEntity(idx);
                if (availableDropZone) {
                    entity.Replace(new DropZone());
                } else {
                    if (entity.Has<DropZone>()) {
                        entity.Del<DropZone>();
                    }
                }
            }
        }
    }
}
