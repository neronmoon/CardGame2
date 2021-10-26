using System;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;

namespace Sources.ECS.Movement {
    public class DefinePlayerAvailableMovesSystem : IEcsRunSystem {
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

                // Player can move only one row above and only to sibling cards
                bool availableDropZone = Math.Abs(runtimeData.PlayerPosition.X - levelPosition.X) < 2 &&
                                         levelPosition.Y == runtimeData.PlayerPosition.Y + 1;
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
