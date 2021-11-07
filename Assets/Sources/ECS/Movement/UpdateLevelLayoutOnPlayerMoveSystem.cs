using System;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using UnityEngine;

namespace Sources.ECS.Movement {
    public class UpdateLevelLayoutOnPlayerMoveSystem : IEcsRunSystem {
        /// <summary>
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;
        private Configuration configuration;
        private EcsFilter<PlayableCard, LevelPosition>.Exclude<Leftover> cards;

        public void Run() {
            foreach (int idx in cards) {
                LevelPosition pos = cards.Get2(idx);
                EcsEntity entity = cards.GetEntity(idx);

                if (entity.Has<Discarded>()) {
                    runtimeData.LevelLayout[pos.Y][pos.X] = null;
                } else if (entity.Has<Player>()) {
                    runtimeData.LevelLayout[pos.Y][pos.X] = configuration.CharacterData;
                }
                
            }
        }
    }
}
