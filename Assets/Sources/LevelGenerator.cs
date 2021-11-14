using System;
using System.Collections.Generic;
using System.Linq;
using Sources.Data.Gameplay;
using Sources.Data.Gameplay.Items;
using Sources.ECS.Extensions;
using UnityEngine;

namespace Sources {
    public class LevelGenerator {
        private System.Random random = new();

        public object[][] Generate(LevelData levelDataSpec, CharacterData characterData, object exit = null) {
            // layout is matrix (N+2)xM where N is level length and M is width of level
            // fist line is for player, last line for exit
            // layout contains object of player/enemies/exits
            // null means empty space without card
            object[][] layout = new object[levelDataSpec.Length + 2][];
            for (int i = 0; i < layout.Length; i++) {
                layout[i] = new object[levelDataSpec.Width];
            }

            // placing player (always in middle of row)
            int center = Mathf.FloorToInt(levelDataSpec.Width / 2);
            layout[0][center] = characterData;

            for (int i = 1; i < layout.Length - 1; i++) {
                int rowWidth = Choose(levelDataSpec.RowWidthChances);
                layout[i] = GenerateRow(levelDataSpec, rowWidth, i, layout[i - 1]);
            }

            // placing exit at last row
            layout[^1][center] = exit ?? levelDataSpec.Exits.ChooseOne();
            return layout;
        }

        private object[] GenerateRow(LevelData levelDataSpec, int width, int posY, object[] previousRow) {
            IEnumerable<bool> layout = null;
            while (layout == null) {
                layout = GenerateRowLayout(Math.Max(width--, 1), previousRow);
            }

            int x = -1;
            object[] row = new object[3];
            foreach (bool item in layout) {
                x++;
                if (!item) continue;

                Type type = Choose(levelDataSpec.CardTypesChances);
                if (type.IsAssignableFrom(typeof(EnemyData))) {
                    row[x] = Choose(levelDataSpec.EnemiesChances);
                } else if (type.IsAssignableFrom(typeof(ChestData))) {
                    row[x] = Choose(levelDataSpec.ChestChances);
                } else if (type.IsAssignableFrom(typeof(ItemData))) {
                    row[x] = Choose(levelDataSpec.ItemChances);
                } else {
                    Debug.LogWarning("Unknown card type!");
                }
            }

            return row;
        }

        private IEnumerable<bool> GenerateRowLayout(int width, object[] previousRow) {
            const int maxWidth = 3;
            bool[] template = new bool[maxWidth];
            for (int i = 0; i < width; i++) {
                template[i] = true;
            }

            bool[] prevRowTemplate = new bool[maxWidth];
            for (int i = 0; i < maxWidth; i++) {
                prevRowTemplate[i] = previousRow[i] != null;
            }

            List<IEnumerable<bool>> finalLayouts = new();
            foreach (IEnumerable<bool> layout in template.GetPermutations()) {
                bool[] candidate = layout.ToArray();
                int withSiblings = 0;
                int items = 0;
                for (int i = 0; i < prevRowTemplate.Length; i++) {
                    if (!prevRowTemplate[i]) continue;
                    items++;
                    bool hasSibling = false;
                    for (int s = Math.Max(0, i - 1); s <= Math.Min(maxWidth - 1, i + 1); s++) {
                        if (candidate[s]) {
                            hasSibling = true;
                        }
                    }

                    if (hasSibling) {
                        withSiblings++;
                    }
                }

                if (items == withSiblings) {
                    finalLayouts.Add(layout);
                }
            }

            return !finalLayouts.Any() ? null : finalLayouts[random.Next(finalLayouts.Count)];
        }

        private Type Choose(IEnumerable<CardTypeChanceData> items) {
            List<CardTypeChanceData> sortedItems = items.ToList();
            sortedItems.Sort((x, xx) => x.Chance - xx.Chance);
            int max = sortedItems.Max(x => x.Chance);

            int c = random.Next(0, max);
            int prev = 0;
            foreach (CardTypeChanceData item in sortedItems) {
                if (c >= prev && c <= item.Chance + prev) {
                    return item.Data;
                }

                prev += item.Chance;
            }

            Debug.LogError("Wrong setup of " + typeof(CardTypeChanceData) + " drop chances");
            return default;
        }

        private T Choose<T>(IEnumerable<ChanceData<T>> items) {
            List<ChanceData<T>> sortedItems = items.ToList();
            sortedItems.Sort((x, xx) => x.Chance - xx.Chance);
            int max = sortedItems.Max(x => x.Chance);
            int prev = 0;
            int c = random.Next(0, max);
            foreach (ChanceData<T> item in sortedItems) {
                if (c >= prev && c <= item.Chance + prev) {
                    return item.Data;
                }

                prev += item.Chance;
            }

            Debug.LogError("Wrong setup of " + typeof(T) + " drop chances");
            return default;
        }
    }
}
