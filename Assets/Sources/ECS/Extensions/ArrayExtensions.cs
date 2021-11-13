using System;
using System.Collections.Generic;
using System.Linq;

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

        // Permutations calculation
        public static IEnumerable<T>[] GetPermutations<T>(this IEnumerable<T> enumerable) {
            T[] array = enumerable as T[] ?? enumerable.ToArray();

            long[] factorials = Enumerable.Range(0, array.Length + 1)
                                          .Select(Factorial)
                                          .ToArray();

            List<IEnumerable<T>> permutations = new();
            for (long i = 0L; i < factorials[array.Length]; i++) {
                int[] sequence = GenerateSequence(i, array.Length - 1, factorials);

                permutations.Add(GeneratePermutation(array, sequence));
            }

            return permutations.ToArray();
        }

        private static IEnumerable<T> GeneratePermutation<T>(T[] array, IReadOnlyList<int> sequence) {
            T[] clone = (T[])array.Clone();

            for (int i = 0; i < clone.Length - 1; i++) {
                Swap(ref clone[i], ref clone[i + sequence[i]]);
            }

            return clone;
        }

        private static int[] GenerateSequence(long number, int size, IReadOnlyList<long> factorials) {
            int[] sequence = new int[size];

            for (int j = 0; j < sequence.Length; j++) {
                long facto = factorials[sequence.Length - j];

                sequence[j] = (int)(number / facto);
                number = (int)(number % facto);
            }

            return sequence;
        }

        private static void Swap<T>(ref T a, ref T b) {
            (a, b) = (b, a);
        }

        private static long Factorial(int n) {
            long result = n;
            for (int i = 1; i < n; i++) {
                result *= i;
            }

            return result;
        }
    }
}
