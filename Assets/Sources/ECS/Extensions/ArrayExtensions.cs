using System;

namespace Sources.ECS.Extensions {
    public static class ArrayExtensions {
        private static Random random = new();

        public static object ChooseOne(this Array objects, bool allowNulls = false) {
            if (objects.Length <= 0) {
                return null;
            }

            object value = objects.GetValue(random.Next(0, objects.Length));
            // TODO: Fix infinite cycle if all nulls
            while (value == null && !allowNulls) {
                value = objects.GetValue(random.Next(0, objects.Length));
            }

            return value;
        }
    }
}
