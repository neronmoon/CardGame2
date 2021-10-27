using System;
using System.Reflection;
using Leopotam.Ecs;

namespace Sources.ECS.Extensions {
    public static class EntityExtensions {
        public static void Del(in this EcsEntity entity, Type type) {
            GetMethod("Del", type).Invoke(null, new object[] { entity });
        }

        private static MethodInfo GetMethod(string name, Type type) {
            MethodInfo method = typeof(EcsEntityExtensions).GetMethod(name);
            return method.MakeGenericMethod(type);
        }
    }
}
