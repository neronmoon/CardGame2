using System.Collections.Generic;
using System.Linq;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.GameplayActions.Components;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = UnityEngine.Input;

namespace Sources.ECS.BaseInteractions {
    public class InputSystem : IEcsRunSystem {
        /// <summary>
        /// This is system for updating global runtime data's input and handle clicks and hovers components
        /// </summary>
        private RuntimeData runtimeData;

        private EcsWorld world;

        private EcsFilter<Interactive, VisualObject> interactive;
        private EcsFilter<StepInProgress> stepInProgress;
        private Camera camera;

        private const float DoubleClickTime = 0.2f;
        private float lastClickTime = 0f;

        public void Run() {
            if (!stepInProgress.IsEmpty()) {
                return;
            }

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

            foreach (int idx in interactive) {
                if (!interactive.Get1(idx).Hoverable) {
                    continue;
                }
                EcsEntity entity = interactive.GetEntity(idx);
                bool alreadyHovered = entity.Has<Hovered>();
                bool shouldBeHovered = !IsOverUI() && size > 0 && hits.Any(hit => {
                    GameObject obj = interactive.Get2(idx).Object;
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

        private bool IsOverUI() {
            PointerEventData pointerData = new(EventSystem.current) {
                position = runtimeData.Input.Position
            };

            List<RaycastResult> results = new();
            EventSystem.current.RaycastAll(pointerData, results);

            if (results.Count > 0) {
                foreach (RaycastResult t in results) {
                    if (t.gameObject.GetComponent<CanvasRenderer>()) {
                        return true;
                    }
                }
            }

            return false;
        }

        private void HandleClicks() {
            bool keyDown = runtimeData.Input.Primary;

            foreach (int idx in interactive) {
                if (!interactive.Get1(idx).Clickable) {
                    continue;
                }
                EcsEntity entity = interactive.GetEntity(idx);
                bool alreadyClicked = entity.Has<Clicked>();
                if (keyDown && entity.Has<Hovered>() && !alreadyClicked) {
                    if (interactive.Get1(idx).DoubleClickable && !entity.Has<DoubleClickedEvent>() && Time.time - lastClickTime <= DoubleClickTime) {
                        entity.Replace(new DoubleClickedEvent());
                    }

                    entity.Replace(new Clicked());
                }

                if (!keyDown && alreadyClicked) {
                    entity.Del<Clicked>();
                    if (interactive.Get1(idx).DoubleClickable) {
                        lastClickTime = Time.time;
                    }
                }
            }
        }
    }
}
