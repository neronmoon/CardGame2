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

        public object[][] Generate<T>(CardsContainer<T> container, Character character, object exit = null) where T : IDataObject, new() {
            // layout is matrix (N+2)xM where N is level length and M is width of level
            // fist line is for player, last line for exit
            // layout contains object of player/enemies/exits
            // null means empty space without card
            object[][] layout = new object[container.Length + 2][];
            for (int i = 0; i < layout.Length; i++) {
                layout[i] = new object[container.Width];
            }

            // placing player (always in middle of row)
            bool isChest = container is Chest;

            int center = Mathf.FloorToInt(container.Width / 2);
            layout[0][center] = character;

            for (int i = 1; i < layout.Length - (isChest ? 1 : 2); i++) {
                int rowWidth = Choose(container.Chances<RowWidth>()).Value;
                layout[i] = GenerateRow(container, rowWidth, i, layout[i - 1]);
            }

            if (!isChest) {
                layout[^2][center] = Choose(container.Chances<Enemy>().Where(x => x.Key.Strongness == Strongness.Boss));
                if (container is Level level && layout[^2][center] is ICanIncreaseValues values) {
                    values.IncreaseValues(level.Difficulty);
                }
            }

            layout[^1][center] = exit ?? Choose(container.Chances<Level>());
            return layout;
        }

        private object BuildCard<T>(CardsContainer<T> container, Type cardType, Strongness strongness) where T : IDataObject, new() {
            object card = null;
            if (cardType.IsAssignableFrom(typeof(Enemy))) {
                card = Choose(container.Chances<Enemy>().Where(e => e.Key.Strongness == strongness));
            } else if (cardType.IsAssignableFrom(typeof(Chest))) {
                card = Choose(container.Chances<Chest>().Where(e => e.Key.Strongness == strongness));
            } else if (cardType.IsAssignableFrom(typeof(Item))) {
                card = Choose(container.Chances<Item>().Where(e => e.Key.Strongness == strongness));
            } else {
                Debug.LogWarning("Unknown card type!");
            }

            if (container is Level level && card is ICanIncreaseValues values) {
                values.IncreaseValues(level.Difficulty);
            }

            return card;
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

                row[x] = BuildCard(level, choices[c].Key, choices[c].Value);
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
                            new(item, Strongness.Easy), new(item, Strongness.Easy), new(item, Strongness.Easy),
                            new(item, Strongness.Hard), new(item, Strongness.Easy), new(item, Strongness.Easy),
                            new(item, Strongness.Hard), new(item, Strongness.Hard), new(item, Strongness.Easy),
                            new(item, Strongness.Hard), new(item, Strongness.Hard), new(item, Strongness.Hard),
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
                        new KeyValuePair<Type, Strongness>[] { new(item, Strongness.Easy), },
                        new KeyValuePair<Type, Strongness>[] { new(item, Strongness.Hard), },
                        new KeyValuePair<Type, Strongness>[] { new(enemy, Strongness.Easy), },
                        new KeyValuePair<Type, Strongness>[] { new(enemy, Strongness.Hard), },
                        new KeyValuePair<Type, Strongness>[] { new(chest, Strongness.Easy), },
                        new KeyValuePair<Type, Strongness>[] { new(chest, Strongness.Hard), }
                    };
                    break;
                case 2:
                    choice = new[] {
                        // enemy - chest
                        new KeyValuePair<Type, Strongness>[] { new(enemy, Strongness.Easy), new(chest, Strongness.Easy), },
                        new KeyValuePair<Type, Strongness>[] { new(enemy, Strongness.Hard), new(chest, Strongness.Hard), },
                        // enemy - enemy
                        new KeyValuePair<Type, Strongness>[] { new(enemy, Strongness.Hard), new(enemy, Strongness.Hard), },
                        // enemy - enemy
                        new KeyValuePair<Type, Strongness>[] { new(enemy, Strongness.Easy), new(enemy, Strongness.Easy), },
                        // enemy - item
                        new KeyValuePair<Type, Strongness>[] { new(enemy, Strongness.Easy), new(item, Strongness.Easy), },
                        // strong item or easy fight
                        new KeyValuePair<Type, Strongness>[] { new(enemy, Strongness.Easy), new(item, Strongness.Hard), },
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
                        new KeyValuePair<Type, Strongness>[] {
                            new(item, Strongness.Easy), new(item, Strongness.Easy), new(item, Strongness.Easy),
                        },
                        // new KeyValuePair<Type, Strongness>[] {
                            // new(item, Strongness.Hard), new(item, Strongness.Hard), new(item, Strongness.Hard),
                        // },
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

            int max = sortedItems.Sum(x => x.Value);
            int prev = 0;
            int c = random.Next(0, max);
            foreach (KeyValuePair<T, int> item in sortedItems) {
                if (c >= prev && c <= item.Value + prev) {
                    // Debug.Log($"max={max} c={c}");
                    return item.Key;
                }

                prev += item.Value;
            }

            Debug.LogError("Wrong setup of " + typeof(T) + " drop chances");
            return default;
        }
    }
}
