using System;
using System.Collections.Generic;
using System.Linq;
using Sources.Data.Gameplay;
using Sources.Database;
using Sources.Database.DataObject;
using Sources.ECS.Extensions;
using UnityEngine;

namespace Sources {
    public class LevelGenerator {
        private System.Random random = new();

        public object[][] Generate<T>(CardsContainer<T> level, Character character, object exit = null) where T : IDataObject, new() {
            // layout is matrix (N+2)xM where N is level length and M is width of level
            // fist line is for player, last line for exit
            // layout contains object of player/enemies/exits
            // null means empty space without card
            object[][] layout = new object[level.Length + 2][];
            for (int i = 0; i < layout.Length; i++) {
                layout[i] = new object[level.Width];
            }

            // placing player (always in middle of row)
            int center = Mathf.FloorToInt(level.Width / 2);
            layout[0][center] = character;

            for (int i = 1; i < layout.Length - 1; i++) {
                int rowWidth = Choose(level.Chances<RowWidth>()).Value;
                layout[i] = GenerateRow(level, rowWidth, i, layout[i - 1]);
            }

            // TODO: place boss at last row
            // placing exit at last row
            layout[^1][center] = exit ?? Choose(level.Chances<Level>());
            return layout;
        }

        private object[] GenerateRow<T>(CardsContainer<T> level, int width, int posY, object[] previousRow) where T : IDataObject, new() {
            IEnumerable<bool> layout = null;
            while (layout == null) {
                layout = GenerateRowLayout(Math.Max(width, 1), previousRow);
                if (layout == null) {
                    width--;
                }
            }

            KeyValuePair<Type, Strongness>[] choices = level is Chest ? InterestingChestChoices(width) : InterestingChoices(width);

            int x = -1;
            int c = -1;
            object[] row = new object[3];
            foreach (bool item in layout) {
                x++;
                if (!item) continue;
                c++;

                Type type = choices[c].Key;
                if (type.IsAssignableFrom(typeof(Enemy))) {
                    row[x] = Choose(level.Chances<Enemy>().Where(e => e.Key.Strongness == choices[c].Value));
                } else if (type.IsAssignableFrom(typeof(Chest))) {
                    row[x] = Choose(level.Chances<Chest>().Where(e => e.Key.Strongness == choices[c].Value));
                } else if (type.IsAssignableFrom(typeof(Item))) {
                    row[x] = Choose(level.Chances<Item>().Where(e => e.Key.Strongness == choices[c].Value));
                } else {
                    Debug.LogWarning("Unknown card type!");
                }
            }

            return row;
        }

        private KeyValuePair<Type, Strongness>[] InterestingChestChoices(int width) {
            Type item = typeof(Item);

            KeyValuePair<Type, Strongness>[][] choice = Array.Empty<KeyValuePair<Type, Strongness>[]>();

            switch (width) {
                case 1: // no choice
                    choice = new[] {
                        new KeyValuePair<Type, Strongness>[] {
                            new(item, Strongness.Hard),
                        }
                    };
                    break;
                case 2:
                    choice = new[] {
                        new KeyValuePair<Type, Strongness>[] {
                            new(item, Strongness.Easy), new(item, Strongness.Easy),
                        },
                        new KeyValuePair<Type, Strongness>[] {
                            new(item, Strongness.Hard), new(item, Strongness.Hard),
                        },
                    };
                    break;
                case 3:
                    choice = new[] {
                        new KeyValuePair<Type, Strongness>[] {
                            new(item, Strongness.Hard), new(item, Strongness.Hard), new(item, Strongness.Hard),
                            new(item, Strongness.Easy), new(item, Strongness.Easy), new(item, Strongness.Easy),
                            new(item, Strongness.Hard), new(item, Strongness.Easy), new(item, Strongness.Easy),
                            new(item, Strongness.Hard), new(item, Strongness.Hard), new(item, Strongness.Easy),
                        },
                    };
                    break;
                default:
                    Debug.LogError("Cannot build interesting choice with width=" + width);
                    break;
            }

            return choice[random.Next(choice.Length)];
        }

        private KeyValuePair<Type, Strongness>[] InterestingChoices(int width) {
            Type enemy = typeof(Enemy);
            Type item = typeof(Item);
            Type chest = typeof(Chest);

            KeyValuePair<Type, Strongness>[][] choice = Array.Empty<KeyValuePair<Type, Strongness>[]>();

            switch (width) {
                case 1: // no choice
                    choice = new[] {
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy),
                        },
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Hard),
                        }
                    };
                    break;
                case 2:
                    choice = new[] {
                        // enemy - chest
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy), new(chest, Strongness.Easy),
                        },
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy), new(chest, Strongness.Hard),
                        },
                        // enemy - enemy
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy), new(enemy, Strongness.Hard),
                        },
                        // enemy - item
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy), new(item, Strongness.Easy),
                        },
                        new KeyValuePair<Type, Strongness>[] { // strong item or easy fight
                            new(enemy, Strongness.Easy), new(item, Strongness.Hard),
                        },
                    };
                    break;
                case 3:
                    choice = new[] {
                        // enemy - enemy - chest
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy), new(enemy, Strongness.Easy), new(chest, Strongness.Easy),
                        },
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy), new(enemy, Strongness.Hard), new(chest, Strongness.Easy),
                        },
                        // enemy - enemy - enemy
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy), new(enemy, Strongness.Easy), new(enemy, Strongness.Hard),
                        },
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy), new(enemy, Strongness.Hard), new(enemy, Strongness.Hard),
                        },
                        // enemy - enemy - item
                        new KeyValuePair<Type, Strongness>[] { // strong item or easy fight
                            new(enemy, Strongness.Easy), new(enemy, Strongness.Easy), new(item, Strongness.Hard),
                        },
                        new KeyValuePair<Type, Strongness>[] {
                            new(enemy, Strongness.Easy), new(enemy, Strongness.Easy), new(item, Strongness.Easy),
                        },
                    };
                    break;
                default:
                    Debug.LogError("Cannot build interesting choice with width=" + width);
                    break;
            }

            return choice[random.Next(choice.Length)];
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

        private T Choose<T>(IEnumerable<KeyValuePair<T, int>> items) {
            List<KeyValuePair<T, int>> sortedItems = items.ToList();
            sortedItems.Sort((x, xx) => x.Value - xx.Value);
            if (sortedItems.Count < 1) {
                Debug.LogError("Chances of " + typeof(T) + " is empty!");
                return default;
            }

            int max = sortedItems.Max(x => x.Value);
            int prev = 0;
            int c = random.Next(0, max);
            foreach (KeyValuePair<T, int> item in sortedItems) {
                if (c >= prev && c <= item.Value + prev) {
                    return item.Key;
                }

                prev += item.Value;
            }

            Debug.LogError("Wrong setup of " + typeof(T) + " drop chances");
            return default;
        }
    }
}
