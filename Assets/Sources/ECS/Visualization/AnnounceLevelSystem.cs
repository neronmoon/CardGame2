using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components.Events;

namespace Sources.ECS.Visualization {
    public class AnnounceLevelSystem : IEcsRunSystem {
        /// <summary>
        /// This system triggers level announce view when new level started
        /// </summary>
        private EcsWorld world;

        private EcsFilter<StartLevelEvent> filter;

        private SceneData sceneData;

        public void Run() {
            foreach (int idx in filter) {
                string levelName = filter.Get1(idx).LevelData.Name;
                sceneData.LevelAnnounceView.AnnounceLevel(levelName);
            }
        }
    }
}
