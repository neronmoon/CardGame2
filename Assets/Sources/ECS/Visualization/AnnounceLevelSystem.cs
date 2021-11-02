using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components.Events;
using UnityEngine;

namespace Sources.ECS.Visualization {
    public class AnnounceLevelSystem : IEcsRunSystem {
        /// <summary>
        /// </summary>
        private EcsWorld world;

        private EcsFilter<StartLevelEvent> filter;

        private SceneData sceneData;

        public void Run() {
            foreach (var idx in filter) {
                string levelName = filter.Get1(idx).Level.Name;
                sceneData.LevelAnnounceView.AnnounceLevel(levelName);
                Debug.Log("Announce level!");
            }
        }
    }
}
