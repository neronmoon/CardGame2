using System;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.Unity;
using Sources.Unity.Support;
using UnityEngine;

namespace Sources.ECS.WorldInitialization {
    public class SpawnCardsGameObjectsSystem : IEcsRunSystem {
        /// <summary>
        /// Spawns cards game objects according to level layout entities and current player position
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;
        private SceneData sceneData;
        private Configuration configuration;
        private ObjectPool pool;
        private EcsFilter<PlayableCard, LevelPosition>.Exclude<VisualObject> cards;

        public void Run() {
            foreach (int idx in cards) {
                LevelPosition pos = cards.Get2(idx);

                // Spawn three rows + player row
                if (Math.Abs(runtimeData.PlayerPosition.Y - pos.Y) >= 4) {
                    continue;
                }

                EcsEntity entity = cards.GetEntity(idx);
                GameObject obj = spawnCardGameObject(entity);
                entity.Replace(new VisualObject { Object = obj });
                entity.Replace(new Spawned());
            }
        }

        private GameObject spawnCardGameObject(EcsEntity entity) {
            GameObject obj = pool.Spawn(configuration.CardPrefab);
            obj.transform.position = sceneData.SpawnPoint.transform.position;
            CardView view = obj.GetComponent<CardView>();
            if (entity.Has<Player>()) {
                view.AdditionalSortOrder = 100;
            }

            view.Sprite.sprite = entity.Has<Face>() ? entity.Get<Face>().Sprite : null;

            foreach (GameObject o in view.HealthObjects) {
                o.SetActive(entity.Has<Health>());
            }

            if (entity.Has<Health>()) {
                view.HealthText.text = entity.Get<Health>().Amount.ToString();
            }

            foreach (GameObject o in view.NameObjects) {
                o.SetActive(entity.Has<Name>());
            }

            if (entity.Has<Name>()) {
                view.NameText.text = entity.Get<Name>().Value;
            }

            // view.Sprite.sprite = entity
            return obj;
        }
    }
}
