using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
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

        private float time;

        public void Init() {
            time = Time.time;
        }

        public void Run() {
            if (runtimeData.CurrentLevel == null && Time.time - time > 2) {
                runtimeData.GarbageEntity.Replace(new StartLevelEvent { Level = configuration.StartLevel });
            }

            foreach (int idx in filter) {
                Level newLevel = filter.Get1(idx).Level;
                if (runtimeData.CurrentLevel == newLevel) {
                    continue;
                }

                runtimeData.CurrentLevel = newLevel;
                runtimeData.CurrentPlayerPosition = 0;
            }
        }
    }
}
