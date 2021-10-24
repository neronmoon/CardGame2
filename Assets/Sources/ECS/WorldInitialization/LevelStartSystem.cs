using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
using Sources.ECS.Components.Events;

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
