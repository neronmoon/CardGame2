using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using UnityEngine;

namespace Sources.ECS.BaseInteractions {
    public class DragndropSystem : IEcsRunSystem {
        /// <summary>
        /// System handles drag events and triggers Dragging and DropCandidate components
        /// </summary>
        private EcsWorld world;

        private Camera camera;
        private RuntimeData runtimeData;

        private EcsFilter<Draggable, Clickable, VisualObject> draggables;
        private EcsFilter<DropZone, VisualObject> dropZones;

        private ContactFilter2D contactFilter = new ContactFilter2D();
        private Plane plane = new Plane(Vector3.forward, new Vector3(0, 0, 0));

        public void Run() {
            foreach (int idx in draggables) {
                EcsEntity entity = draggables.GetEntity(idx);
                bool dragging = entity.Has<Dragging>();
                bool clicked = entity.Has<Clicked>();

                if (!dragging && clicked) {
                    entity.Replace(Grabbed(entity));
                }

                if (dragging && clicked) {
                    Dragging(entity);
                }

                if (dragging && !clicked) {
                    Released(entity);
                    entity.Del<Dragging>();
                }
            }
        }

        private Dragging Grabbed(EcsEntity entity) {
            Vector3 pointer = GetWorldPosition(runtimeData.Input.Position);
            Vector3 startPosition = entity.Get<VisualObject>().Object.transform.position;
            return new Dragging {
                StartPosition = startPosition,
                Offset = startPosition - pointer
            };
        }

        private void Dragging(EcsEntity entity) {
            Vector3 pos = GetWorldPosition(runtimeData.Input.Position) + entity.Get<Dragging>().Offset;
            entity.Get<VisualObject>().Object.transform.position = pos;

            EcsEntity? candidate = getDropZoneCandidate(entity);
            foreach (int idx in dropZones) {
                EcsEntity zone = dropZones.GetEntity(idx);
                bool alreadyCandidate = zone.Has<DropCandidate>();
                bool shouldBeCandidate = zone == candidate;
                if (shouldBeCandidate && !alreadyCandidate) {
                    zone.Replace(new DropCandidate());
                }

                if (!shouldBeCandidate && alreadyCandidate) {
                    zone.Del<DropCandidate>();
                }
            }
        }

        private void Released(EcsEntity entity) {
            GameObject gameObject = entity.Get<VisualObject>().Object;
            EcsEntity? candidate = getDropZoneCandidate(entity);

            Vector3 pos = candidate != null
                ? ((EcsEntity)candidate).Get<VisualObject>().Object.transform.position
                : entity.Get<Dragging>().StartPosition;

            if (candidate != null) {
                runtimeData.GarbageEntity.Replace(new DroppedEvent { DropZone = (EcsEntity)candidate });
            }

            gameObject.transform.DOMove(pos, 0.1f);
        }

        private EcsEntity? getDropZoneCandidate(EcsEntity entity) {
            GameObject gameObject = entity.Get<VisualObject>().Object;

            List<Collider2D> overlaps = new List<Collider2D>();
            int count = gameObject.GetComponent<Collider2D>().OverlapCollider(contactFilter, overlaps);
            if (count <= 0) {
                return null;
            }

            float distance = 10000;
            EcsEntity candidate = default;
            foreach (int idx in dropZones) {
                GameObject dropzoneObj = dropZones.Get2(idx).Object;
                bool isOverlapping = overlaps.Any(coll => coll.gameObject == dropzoneObj);
                if (!isOverlapping) {
                    continue;
                }

                float d = (dropzoneObj.transform.position - gameObject.transform.position).magnitude;
                if (d < distance) {
                    distance = d;
                    candidate = dropZones.GetEntity(idx);
                }
            }

            return candidate != default ? candidate : (EcsEntity?)null;
        }

        private Vector3 GetWorldPosition(Vector2 screenPosition) {
            Ray ray = camera.ScreenPointToRay(screenPosition);
            plane.Raycast(ray, out float distance);
            return ray.GetPoint(distance);
        }
    }
}
