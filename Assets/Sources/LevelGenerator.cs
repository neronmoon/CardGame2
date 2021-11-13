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

        // public object[][] Generate(LevelData levelDataSpec, CharacterData characterData, object exit = null) {
        //     // layout is matrix (N+2)xM where N is level length and M is width of level
        //     // fist line is for player, last line for exit
        //     // layout contains object of player/enemies/exits
        //     // null means empty space without card
        //     object[][] layout = new object[levelDataSpec.Length + 2][];
        //     for (int i = 0; i < layout.Length; i++) {
        //         layout[i] = new object[levelDataSpec.Width];
        //     }
        //
        //     // placing player (always in middle of row)
        //     int center = Mathf.FloorToInt(levelDataSpec.Width / 2);
        //     layout[0][center] = characterData;
        //
        //     // generating level
        //     for (int i = 1; i < layout.Length - 1; i++) {
        //         int nullsCount = 0;
        //         for (int j = 0; j < layout[i].Length; j++) {
        //             layout[i][j] = new[] { levelDataSpec.Enemies.ChooseOne(), levelDataSpec.Items.ChooseOne(), levelDataSpec.Chests.ChooseOne() }.ChooseOne();
        //             if (layout[i][j] == null) {
        //                 nullsCount++;
        //             }
        //         }
        //     }
        //
        //     // placing exit at last row
        //     layout[^1][center] = exit ?? levelDataSpec.Exits.ChooseOne();
        //     return layout;
        // }

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
            const int maxWidth = 3;
            bool[] template = new bool[maxWidth];
            for (int i = 0; i < width; i++) {
                template[i] = true;
            }

            // var finalLayouts = new List<IEnumerable<bool>>();
            // foreach (IEnumerable<bool> layout in template.GetPermutations()) {
            //     bool allowed = false;
            //
            //     int xIdx = 0;
            //     foreach (var xx in layout) {
            //
            //         if (!xx) {
            //             
            //         }
            //         
            //         xIdx++;
            //     }
            //     
            //     
            //     if (allowed) {
            //         finalLayouts.Add(layout);
            //     }
            // }
            //
            IEnumerable<bool>[] layouts = template.GetPermutations().ToArray();
            if (previousRow[1] == null) { // if prev row middle is empty -- then we allow layouts in non-empty middle
                layouts = layouts.Where(x => {
                    bool[] ar = x.ToArray();
                    return ar[1];
                }).ToArray();
            }

            object[] row = new object[3];
            int x = 0;
            foreach (bool item in (IEnumerable<bool>)layouts.ChooseOne()) {
                object value = null;
                if (item) {
                    Type type = Choose(levelDataSpec.CardTypesChances);
                    if (type.IsAssignableFrom(typeof(EnemyData))) {
                        value = Choose(levelDataSpec.EnemiesChances);
                    } else if (type.IsAssignableFrom(typeof(ChestData))) {
                        value = Choose(levelDataSpec.ChestChances);
                    } else if (type.IsAssignableFrom(typeof(ItemData))) {
                        value = Choose(levelDataSpec.ItemChances);
                    } else {
                        Debug.LogWarning("");
                    }
                }

                row[x++] = value;
            }

            return row;
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
