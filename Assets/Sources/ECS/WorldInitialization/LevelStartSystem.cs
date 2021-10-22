using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components.Events;
using UnityEngine;

namespace Sources.ECS.WorldInitialization {
    public class LevelStartSystem : IEcsInitSystem, IEcsRunSystem {
        /// <summary>
        /// Creates entity with one-frame component to indicate level start
        /// Also changes runtimeData's current level and current player position props
        /// </summary>
        private EcsWorld world;

        private Configuration configuration;
        private RuntimeData runtimeData;

        private EcsFilter<StartLevelEvent> filter;

        public void Init() {
            runtimeData.GarbageEntity.Replace(new StartLevelEvent { Level = configuration.StartLevel });
        }

        public void Run() {
            foreach (var idx in filter) {
                var newLevel = filter.Get1(idx).Level;
                if (runtimeData.CurrentLevel != newLevel) {
                    runtimeData.CurrentLevel = newLevel;
                    runtimeData.CurrentPlayerPosition = 0;
                }

                Debug.Log("Level changed to " + newLevel.Name);
            }
        }
    }
}
