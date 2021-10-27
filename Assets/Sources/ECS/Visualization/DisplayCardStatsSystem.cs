using Leopotam.Ecs;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.Unity;
using UnityEngine;

namespace Sources.ECS.Visualization {
    public class DisplayCardStatsSystem : IEcsRunSystem {
        /// <summary>
        /// This system is reflecting ecs entity data into card view
        /// </summary>
        private EcsWorld world;

        private EcsFilter<PlayableCard, VisualObject> cards;

        public void Run() {
            foreach (var idx in cards) {
                FillStats(cards.GetEntity(idx), cards.Get2(idx).Object);
            }
        }

        private static void FillStats(EcsEntity entity, GameObject obj) {
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
        }
    }
}
