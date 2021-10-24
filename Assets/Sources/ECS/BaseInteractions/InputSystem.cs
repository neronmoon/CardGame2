using System.Linq;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using UnityEngine;
using Input = UnityEngine.Input;

namespace Sources.ECS.BaseInteractions {
    public class InputSystem : IEcsRunSystem {
        /// <summary>
        /// This is system for updating global runtime data's input and handle clicks and hovers components
        /// </summary>
        private RuntimeData runtimeData;

        private EcsWorld world;

        private EcsFilter<Hoverable, Hovered, Clickable, VisualObject> clickables;
        private EcsFilter<Hoverable, VisualObject> hoverables;
        private Camera camera;

        public void Run() {
            runtimeData.Input.Position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            runtimeData.Input.Primary = Input.GetMouseButton(0);
            runtimeData.Input.Secondary = Input.GetMouseButton(1);

            HandleHover();
            HandleClicks();
        }

        private void HandleHover() {
            RaycastHit2D[] hits = new RaycastHit2D[2];
            int size = Physics2D.GetRayIntersectionNonAlloc(
                camera.ScreenPointToRay(runtimeData.Input.Position),
                hits
            );

            foreach (int idx in hoverables) {
                EcsEntity entity = hoverables.GetEntity(idx);
                bool alreadyHovered = entity.Has<Hovered>();
                bool shouldBeHovered = size > 0 && hits.Any(hit => {
                    GameObject obj = hoverables.Get2(idx).Object;
                    return hit && hit.collider.gameObject == obj;
                });
                if (alreadyHovered && !shouldBeHovered) {
                    entity.Del<Hovered>();
                }

                if (!alreadyHovered && shouldBeHovered) {
                    entity.Replace(new Hovered());
                }
            }
        }

        private void HandleClicks() {
            bool keyDown = runtimeData.Input.Primary;

            foreach (int idx in clickables) {
                EcsEntity entity = clickables.GetEntity(idx);
                bool alreadyClicked = entity.Has<Clicked>();
                if (keyDown && !alreadyClicked) {
                    entity.Replace(new Clicked());
                }

                if (!keyDown && alreadyClicked) {
                    entity.Del<Clicked>();
                }
            }
        }
    }
}
