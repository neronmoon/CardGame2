using System;
using System.Reflection;
using Leopotam.Ecs;
using UnityEngine;

namespace Sources.ECS.Extensions {
    public static class EntityExtensions {
        public static void Del(in this EcsEntity entity, Type type) {
            GetMethod("Del", type).Invoke(null, new object[] { entity });
        }

        public static void Replace(in this EcsEntity entity, object component) {
            Type type = component.GetType();
            if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null) {
                Debug.LogError($"Trying to *magically* replace component of type {type}, that is not struct!");
            }

            GetMethod("Replace", type).Invoke(null, new[] { entity, component });
        }

        private static MethodInfo GetMethod(string name, Type type) {
            MethodInfo method = typeof(EcsEntityExtensions).GetMethod(name);
            return method.MakeGenericMethod(type);
        }
    }
}
