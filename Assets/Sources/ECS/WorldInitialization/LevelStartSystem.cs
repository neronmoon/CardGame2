using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
using Sources.ECS.Components.Events;
using Sources.ECS.GameplayActions.Components;
using UnityEngine;

namespace Sources.ECS.WorldInitialization {
    public class LevelStartSystem : IEcsInitSystem, IEcsRunSystem {
        /// <summary>
        /// Creates entity with one-frame component to indicate level start
        /// Also changes runtimeData's current level
        /// </summary>
        private EcsWorld world;

        private Configuration configuration;
        private RuntimeData runtimeData;

        private EcsFilter<StartLevelEvent> start;
        private EcsFilter<LevelChange> change;

        private float time;

        public void Init() {
            time = Time.time;
        }

        public void Run() {
            // Add a bit delay, because animations are freezing at start
            // TODO: Fix this!
            if (runtimeData.CurrentLevel == null && Time.time - time > 1) {
                runtimeData.GarbageEntity.Replace(new StartLevelEvent { Level = configuration.StartLevel });
            }

            foreach (var idx in change) {
                runtimeData.GarbageEntity.Replace(new StartLevelEvent { Level = change.Get1(idx).Level });
            }

            foreach (int idx in start) {
                Level level = start.Get1(idx).Level;
                if (runtimeData.CurrentLevel != level) {
                    runtimeData.CurrentLevel = level;
                }
            }
        }
    }
}
