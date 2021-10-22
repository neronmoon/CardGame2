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
        private Configuration configuration;
        private ObjectPool pool;
        private EcsFilter<PlayableCard, LevelPosition>.Exclude<VisualObject> cards;

        private const int VisibleRows = 3;

        public void Run() {
            foreach (int idx in cards) {
                LevelPosition pos = cards.Get2(idx);
                if (runtimeData.CurrentPlayerPosition - pos.Y >= VisibleRows) continue;
                EcsEntity entity = cards.GetEntity(idx);
                GameObject obj = spawnCardGameObject(entity);
                entity.Replace(new VisualObject { Object = obj });
            }
        }

        private GameObject spawnCardGameObject(EcsEntity entity) {
            GameObject obj = pool.Spawn(configuration.CardPrefab);
            return obj;
        }
    }
}
