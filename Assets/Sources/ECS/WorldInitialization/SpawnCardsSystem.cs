using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.Unity.Support;
using UnityEngine;

namespace Sources.ECS.WorldInitialization {
    public class SpawnCardsSystem : IEcsRunSystem {
        /// <summary>
        /// Spawn cards according to level layout from runtime data
        /// </summary>
        private EcsWorld world;

        private Configuration configuration;
        private RuntimeData runtimeData;
        private ObjectPool pool;

        private EcsFilter<StartLevelEvent> startFilter;

        public void Run() {
            if (startFilter.IsEmpty()) return;

            object[][] layout = runtimeData.LevelLayout;
            for (int i = 0; i < layout.Length; i++) {
                Debug.Log("LINE " + i + "----------");
                for (int j = 0; j < layout[i].Length; j++) {
                    Debug.Log(layout[i][j]);
                }
            }
            //
            // EcsEntity playerEntity = world.NewEntity();
            // playerEntity.Replace(new Player());
            // GameObject playerObj = pool.Spawn(configuration.CardPrefab);
            // playerObj.tag = "Entity";
            // playerObj.name = "Player";
            // playerEntity.Replace(new VisualObject { Object = playerObj });
            // playerEntity.Replace(new Hoverable());
            // playerEntity.Replace(new Clickable());
        }
    }
}
