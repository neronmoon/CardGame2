using System.Collections.Generic;
using System.Linq;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
using Sources.Database.DataObject;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.Unity;
using UnityEngine;
using Enemy = Sources.Database.DataObject.Enemy;

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
        private EcsFilter<Player, LevelPosition, Inventory> player;

        public void Run() {
            if (levelChanged.IsEmpty() && movement.IsEmpty()) {
                return;
            }
            bool hasMap = false;
            foreach (int idx in player) {
                hasMap = player.Get3(idx).HasItem("Map");
            }

            sceneData.LevelMapContainer.gameObject.SetActive(hasMap);
            if (!hasMap) {
                return;
            }
            
            RedrawMap();
            foreach (int idx in player) {
                AdjustMapState(player.Get2(idx).Y);
            }
        }

        private void RedrawMap() {
            GameObject prefab = configuration.LevelChunkPrefab;
            Transform container = sceneData.LevelMapContainer;
            foreach (LevelMapChunk component in container.GetComponentsInChildren<LevelMapChunk>()) {
                component.Destroy();
            }

            for (int y = runtimeData.LevelLayout.Length - 1; y >= 0; y--) {
                LevelMapChunk chunk = Object.Instantiate(prefab, container).GetComponent<LevelMapChunk>();
                chunk.SetYPosition(y);
                chunk.SetType(GetRowType(runtimeData.LevelLayout[y]));
            }
        }

        private void AdjustMapState(int playerPosition) {
            Transform container = sceneData.LevelMapContainer;
            foreach (LevelMapChunk component in container.GetComponentsInChildren<LevelMapChunk>()) {
                component.SetState(component.Y >= playerPosition);

                if (component.Y == playerPosition) {
                    component.SetType("player");
                }
            }
        }

        private string GetRowType(object[] row) {
            IEnumerable<object> withoutNulls = row.Where(x => x != null).ToList();

            if (withoutNulls.All(x => x is Item { Strongness: Strongness.Boss })) {
                return "boss";
            }

            if (withoutNulls.All(x => x is Item)) {
                return "item";
            }

            if (withoutNulls.All(x => x is Chest)) {
                return "chest";
            }

            if (withoutNulls.All(x => x is Enemy)) {
                return "fight";
            }

            if (withoutNulls.All(x => x is Level or ChestExit)) {
                return "exit";
            }

            return null;
        }
    }
}
