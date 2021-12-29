using Leopotam.Ecs;
using NaughtyAttributes;
using Sources.Database.DataObject;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.Components.Gameplay.Perks;
using Sources.ECS.Extensions;
using Sources.Unity.Support;
using Sources.Unity.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using Enemy = Sources.ECS.Components.Gameplay.CardTypes.Enemy;

namespace Sources.Unity {
    public class CardDetailsView : View {
        public Transform CardContainer;
        public GameObject LastCardPreview;
        public TextMeshProUGUI DescriptionText;
        public GameObject DetailsLinePrefab;
        public Transform DetailsContainer;
        public EcsStartup Startup;

        [Foldout("Sprites")]
        public Sprite DamageSprite;

        [Foldout("Sprites")]
        public Sprite AggressiveSprite;

        public void SetCard(EcsEntity entity) {
            if (LastCardPreview != null) {
                Destroy(LastCardPreview);
            }

            GameObject prefab = Startup.Configuration.CardPrefab;
            if (entity.Has<Player>()) {
                prefab = Startup.Configuration.PlayerPrefab;
            } else if (entity.Has<ConsumableItem>() || entity.Has<EquippableItem>()) {
                prefab = Startup.Configuration.ItemCardPrefab;
            } else if (entity.Has<LevelEntrance>()) {
                prefab = Startup.Configuration.LevelCardPrefab;
            }

            GameObject obj = Instantiate(prefab, CardContainer);
            obj.GetComponent<CardView>().FillStats(entity);
            AdjustCardViewToPreview(obj);
            FillDescription(entity);
            FillLines(entity);
            LastCardPreview = obj;
        }

        private void FillLines(EcsEntity entity) {
            if (entity.Has<Player>()) {
                //TODO: show inventory
            } else {
                foreach (DetailsLineView child in DetailsContainer.GetComponentsInChildren<DetailsLineView>()) {
                    Destroy(child.gameObject);
                }

                if (entity.Has<Enemy>()) {
                    if (entity.Has<Health>()) {
                        DetailsLineView view = Instantiate(DetailsLinePrefab, DetailsContainer).GetComponent<DetailsLineView>();
                        view.Fill(DamageSprite, $"Deals {entity.Get<Health>().Value} damage");
                    }

                    if (entity.Has<Aggressive>()) {
                        DetailsLineView view = Instantiate(DetailsLinePrefab, DetailsContainer).GetComponent<DetailsLineView>();
                        view.Fill(AggressiveSprite, $"Attacks you first");
                    }
                }
            }
        }

        private void FillDescription(EcsEntity entity) {
            string description = "";

            if (entity.Has<Enemy>()) {
                description = entity.Get<Enemy>().Data.Description;
            } else if (entity.Has<Player>()) {
                description = entity.Get<Player>().Data.Description;
            } else if (entity.Has<ConsumableItem>()) {
                description = entity.Get<ConsumableItem>().Data.Description;
            } else if (entity.Has<EquippableItem>()) {
                description = entity.Get<EquippableItem>().Data.Description;
            } else if (entity.Has<ChestEntrance>()) {
                description = entity.Get<ChestEntrance>().Data.Description;
            } else if (entity.Has<LevelEntrance>()) {
                description = entity.Get<LevelEntrance>().Data.Description;
            }

            DescriptionText.text = description;
        }

        private void AdjustCardViewToPreview(GameObject obj) {
            string UILayer = "UI";
            SetLayer(UILayer);
            foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true)) {
                if (trans.name.Contains("Mask")) {
                    trans.gameObject.SetActive(false);
                }
            }

            foreach (Canvas canvas in obj.GetComponentsInChildren<Canvas>()) {
                canvas.overrideSorting = true;
                canvas.sortingLayerName = UILayer;
            }

            foreach (SortingGroup grp in obj.GetComponentsInChildren<SortingGroup>()) {
                grp.sortingLayerName = UILayer;
            }

            foreach (Renderer renderer in obj.GetComponentsInChildren<Renderer>()) {
                renderer.sortingLayerName = UILayer;
            }
        }
    }
}
