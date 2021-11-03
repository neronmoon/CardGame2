using Leopotam.Ecs;

namespace Sources.ECS.Extensions {
    public static class EcsFilterExtensions {
        public static EcsEntity? First(this EcsFilter filter) {
            foreach (int idx in filter) {
                return filter.GetEntity(idx);
            }

            return null;
        }

        public static T GetComponentOnFirstOrDefault<T>(this EcsFilter filter, T defaultValue) where T : struct {
            EcsEntity? ecsEntity = filter.First();
            return ecsEntity?.Get<T>() ?? defaultValue;
        }
    }
}
