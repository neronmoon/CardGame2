using System;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.Unity.Support;
using UnityEngine;

namespace Sources.ECS.WorldInitialization {
    public class SpawnCardsGameObjectsSystem : IEcsRunSystem {
        /// <summary>
        /// Spawns cards game objects according to level layout entities and current player position
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;
        private SceneData sceneData;
        private Configuration configuration;
        private ObjectPool pool;
        private EcsFilter<PlayableCard, LevelPosition>.Exclude<VisualObject> cards;

        public void Run() {
            foreach (int idx in cards) {
                LevelPosition pos = cards.Get2(idx);
                
                // Spawn three rows + player row
                if (Math.Abs(runtimeData.CurrentPlayerPosition - pos.Y) >= 4) {
                    continue;
                }

                EcsEntity entity = cards.GetEntity(idx);
                GameObject obj = spawnCardGameObject(entity);
                entity.Replace(new VisualObject { Object = obj });
                entity.Replace(new Spawned());
            }
        }

        private GameObject spawnCardGameObject(EcsEntity entity) {
            GameObject obj = pool.Spawn(configuration.CardPrefab);
            obj.transform.position = sceneData.SpawnPoint.transform.position;
            return obj;
        }

    }
}
