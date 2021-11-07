using System;
using System.Collections.Generic;
using System.Linq;
using Sources.Data.Gameplay;
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
                layout[i] = GenerateRow(Choose(levelDataSpec.RowWidthChances), i, layout[i - 1]);
            }

            // placing exit at last row
            layout[^1][center] = exit ?? levelDataSpec.Exits.ChooseOne();
            return layout;
        }

        private object[] GenerateRow(int width, int posY, object[] previousRow) {
            const int maxWidth = 3;
            
            bool[] template = new bool[maxWidth];
            for (int j = 0; j < width; j++) {
                template[j] = true;
            }
            
            IEnumerable<bool>[] layouts = GetPermutations(template, maxWidth).ToArray();
            
            IEnumerable<bool> l = (IEnumerable<bool>)layouts.ChooseOne();
            object[] row = new object[3];
            int i = 0;
            foreach (bool item in l) {
                if (!item) {
                    row[i] = null;
                } else {
                    // choose card type
                    // choose card from selected
                }

                i++;
            }

            return row;
        }

        private IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length) {
            if (length == 1) return list.Select(t => new T[] { t });

            return GetPermutations(list, length - 1)
                .SelectMany(t => list.Where(e => !t.Contains(e)),
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        private T Choose<T>(IEnumerable<ChanceData<T>> items) {
            List<ChanceData<T>> a = items.ToList();
            a.Sort((x, xx) => x.Chance - xx.Chance);

            int prevChance = 0;
            int c = random.Next(0, 100);
            foreach (ChanceData<T> item in a) {
                if (c >= prevChance && c <= item.Chance + prevChance) {
                    return item.Data;
                }

                prevChance += item.Chance;
            }

            return default;
        }
    }
}
