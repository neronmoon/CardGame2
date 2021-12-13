using Leopotam.Ecs;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.Unity.Support;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sources.Unity {
    public class CardDetailsView : View {
        public Transform CardContainer;
        public GameObject LastCardPreview;
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
            LastCardPreview = obj;
        }

        private static void AdjustCardViewToPreview(GameObject obj) {
            string UILayer = "UI";
            foreach (Transform trans in obj.GetComponentsInChildren<Transform>(true)) {
                if (trans.name.Contains("Mask")) {
                    trans.gameObject.SetActive(false);
                } else {
                    trans.gameObject.layer = LayerMask.NameToLayer(UILayer);
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
