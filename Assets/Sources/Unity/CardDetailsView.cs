using Leopotam.Ecs;
using Sources.Database.DataObject;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.Extensions;
using Sources.Unity.Support;
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
            LastCardPreview = obj;
        }

        private void FillDescription(EcsEntity entity) {
            string description = "";
            
            if (entity.Has<Enemy>()) {
                description = entity.Get<Enemy>().Data.Description;
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
