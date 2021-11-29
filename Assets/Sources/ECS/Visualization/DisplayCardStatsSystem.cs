using Leopotam.Ecs;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.Components.Gameplay.Perks;
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
            foreach (int idx in cards) {
                FillStats(cards.GetEntity(idx), cards.Get2(idx).Object);
            }
        }

        private static void FillStats(EcsEntity entity, GameObject obj) {
            CardView view = obj.GetComponent<CardView>();
            if (entity.Has<Player>()) {
                view.AdditionalSortOrder = 100;
            }

            view.Sprite.sprite = entity.Has<Face>() ? entity.Get<Face>().Sprite : null;

            // TODO: Add icons to display item effects
            foreach (GameObject o in view.ValueObjects) {
                o.SetActive(entity.Has<Health>() || entity.Has<EquippableItem>());
            }

            if (entity.Has<Health>()) {
                view.ValueText.text = entity.Get<Health>().Value.ToString();
            }

            if (entity.Has<EquippableItem>()) {
                view.ValueText.text = entity.Get<EquippableItem>().Data.Count.ToString();
            }

            foreach (GameObject o in view.NameObjects) {
                o.SetActive(entity.Has<Name>());
            }

            if (entity.Has<Name>()) {
                view.NameText.text = entity.Get<Name>().Value;
            }

            view.SetAggressive(entity.Has<Aggressive>());
        }
    }
}
