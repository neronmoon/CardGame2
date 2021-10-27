using DG.Tweening;
using Leopotam.Ecs;
using Sources.ECS.Animations.Components;
using Sources.ECS.Components;

namespace Sources.ECS.Animations {
    public class CleanupAnimatedSystem : IEcsRunSystem {
        /// <summary>
        /// Is card animation system we put `animated` component on entities
        /// In this system we should check if entity is still animated and remove component if animation is completed
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Animated, VisualObject> animations;

        public void Run() {
            foreach (var idx in animations) {
                bool isTweening = DOTween.IsTweening(animations.Get2(idx).Object.transform, true);
                EcsEntity entity = animations.GetEntity(idx);
                if (!isTweening && entity.Has<Animated>()) {
                    entity.Del<Animated>();
                }
            }
        }
    }
}
