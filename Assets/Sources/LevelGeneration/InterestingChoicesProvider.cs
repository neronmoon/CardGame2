using System;
using System.Collections.Generic;
using Sources.Data.Gameplay;
using Sources.Database.DataObject;
using UnityEngine;

namespace Sources.LevelGeneration {
    public class InterestingChoicesProvider {
        
        public static KeyValuePair<Type, Strongness>[][] ForChest(int width) {
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

            return choice;
        }

        public static KeyValuePair<Type, Strongness>[][] ForLevel(int width) {
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

            
            return choice;
        }
    }
}
