using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.Extensions;
using Sources.Unity;
using Unity.VisualScripting;
using UnityEngine;

namespace Sources.ECS.Visualization {
    public class DrawLevelMapSystem : IEcsRunSystem {
        /// <summary>
        /// This system adjusts level map to runtime data's
        /// </summary>
        private Configuration configuration;

        private SceneData sceneData;
        private RuntimeData runtimeData;

        private EcsFilter<StartLevelEvent> levelChanged;
        private EcsFilter<PlayerMovedEvent> movement;
        private EcsFilter<Player, LevelPosition> player;

        public void Run() {
            if (!levelChanged.IsEmpty()) {
                RedrawMap();
                foreach (int idx in player) {
                    AdjustMapState(player.Get2(idx).Y);
                }
            }

            foreach (int idx in movement) {
                AdjustMapState(movement.Get1(idx).Target.Get<LevelPosition>().Y);
            }
        }

        private void RedrawMap() {
            GameObject prefab = configuration.LevelChunkPrefab;
            Transform container = sceneData.LevelMapContainer;
            foreach (LevelMapChunk component in container.GetComponentsInChildren<LevelMapChunk>()) {
                Object.Destroy(component.gameObject);
            }

            for (int y = runtimeData.LevelLayout.Length - 1; y >= 0; y--) {
                LevelMapChunk chunk = Object.Instantiate(prefab, container).GetComponent<LevelMapChunk>();
                chunk.SetYPosition(y);
            }
        }

        private void AdjustMapState(int playerPosition) {
            Transform container = sceneData.LevelMapContainer;
            foreach (LevelMapChunk component in container.GetComponentsInChildren<LevelMapChunk>()) {
                component.SetState(component.Y > playerPosition);
            }
        }
    }
}
