using System;
using DG.Tweening;
using Leopotam.Ecs;
using Sources.ECS.Animations.Components;
using Sources.ECS.Components;
using UnityEngine;

namespace Sources.ECS.Animations {
    public class CleanupAnimatedSystem : IEcsRunSystem {
        /// <summary>
        /// Is card animation system we put `animated` component on entities
        /// In this system we should check if entity is still animated and remove component if animation is completed
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Animated, VisualObject> animations;

        private Type[] tweenableComponents = {
            typeof(Transform),
            typeof(SpriteRenderer),
            typeof(CanvasGroup),
        };

        public void Run() {
            foreach (var idx in animations) {
                GameObject obj = animations.Get2(idx).Object;
                bool isTweening = IsTweening(obj);
                EcsEntity entity = animations.GetEntity(idx);
                if (!isTweening && entity.Has<Animated>()) {
                    entity.Del<Animated>();
                }
            }
        }

        private bool IsTweening(GameObject obj) {
            foreach (Type type in tweenableComponents) {
                Component[] components = obj.GetComponentsInChildren(type);
                foreach (Component component in components) {
                    if (DOTween.IsTweening(component)) {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
