using System;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Extensions;
using Sources.ECS.GameplayActions.Components;
using Sources.Unity;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Sources.ECS.Visualization {
    public class SpawnCardsGameObjectsSystem : IEcsRunSystem {
        /// <summary>
        /// Spawns cards game objects according to level layout entities and current player position
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;
        private SceneData sceneData;
        private Configuration configuration;
        private EcsFilter<PlayableCard, LevelPosition>.Exclude<VisualObject,Discarded> cards;
        private EcsFilter<PlayableCard, LevelPosition, Player> playerCard;

        private EcsFilter<Player, CompleteStep> playerStepCompleted;
        private EcsFilter<StartLevelEvent> levelStared;

        public void Run() {

            // Spawn cards only if player move is finished or level is starting 
            if(playerStepCompleted.IsEmpty() && levelStared.IsEmpty()) return;
            
            if (runtimeData.PlayerIsDead) return;

            EcsEntity? ecsEntity = playerCard.First();
            int playerPosition = ecsEntity?.Get<LevelPosition>().Y ?? 0;
            foreach (int idx in cards) {
                LevelPosition pos = cards.Get2(idx);
                EcsEntity entity = cards.GetEntity(idx);

                // Spawn three rows + player row
                int distance = Math.Abs(playerPosition - pos.Y);
                if (distance is >= 4 or < 1 && !entity.Has<Player>()) {
                    continue;
                }

                GameObject obj = spawnCardGameObject(entity);
                entity.Replace(new VisualObject {Object = obj});
                entity.Replace(new Spawned());
            }
        }

        private GameObject spawnCardGameObject(EcsEntity entity) {
            GameObject obj = Object.Instantiate(configuration.CardPrefab);
            obj.transform.position = sceneData.SpawnPoint.transform.position;
            CardView view = obj.GetComponent<CardView>();
            if (entity.Has<Player>()) {
                view.AdditionalSortOrder = 100;
            }

            return obj;
        }
    }
}
