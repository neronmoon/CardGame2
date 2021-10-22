using System.Linq;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using UnityEngine;

namespace Sources.ECS.BaseInteractions {
    public class HoverSystem : IEcsRunSystem {
        /// <summary>
        /// Adds/removes Hovered component
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;
        private Camera camera;
        private EcsFilter<Hoverable, VisualObject> hoverables;

        public void Run() {
            RaycastHit2D[] rayHits = new RaycastHit2D[2];
            int size = Physics2D.GetRayIntersectionNonAlloc(
                camera.ScreenPointToRay(runtimeData.Input.Position),
                rayHits
            );

            foreach (int idx in hoverables) {
                EcsEntity entity = hoverables.GetEntity(idx);
                bool alreadyHovered = entity.Has<Hovered>();
                bool shouldBeHovered = size > 0 && checkHovered(hoverables.Get2(idx).Object, rayHits);
                if (alreadyHovered && !shouldBeHovered) {
                    entity.Del<Hovered>();
                    Debug.Log("[HoverSystem] Not hovered");
                }

                if (!alreadyHovered && shouldBeHovered) {
                    entity.Replace(new Hovered());
                    Debug.Log("[HoverSystem] Hovered");
                }
            }
        }

        private bool checkHovered(GameObject obj, RaycastHit2D[] hits) {
            return hits.Any(hit => hit.transform.gameObject == obj);
        }
    }
}
