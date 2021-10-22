using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;

namespace Sources.ECS.WorldInitialization {
    public class InitGarbageEntity : IEcsInitSystem {
        /// <summary>
        /// Creates entity for garbage components: events or other
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;

        public void Init() {
            runtimeData.GarbageEntity = world.NewEntity();
            runtimeData.GarbageEntity.Replace(new EventsHolder());
        }
    }
}

