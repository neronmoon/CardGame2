using Leopotam.Ecs;
using Sources.ECS.Animations.Components;
using Sources.ECS.Components;
using Object = UnityEngine.Object;

namespace Sources.ECS.WorldInitialization {
    public class RecycleDiscardedEntitiesSystem : IEcsRunSystem {
        /// <summary>
        /// </summary>
        private EcsWorld world;

        private EcsFilter<VisualObject, Discarded>.Exclude<Animated> objects;

        public void Run() {
            foreach (int idx in objects) {
                VisualObject obj = objects.Get1(idx);
                Object.Destroy(obj.Object);
                objects.GetEntity(idx).Destroy();
            }
        }
    }
}
