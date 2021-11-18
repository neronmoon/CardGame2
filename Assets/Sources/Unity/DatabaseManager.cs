using System.Collections.Generic;
using NaughtyAttributes;
using Sources.Data.Gameplay;
using Sources.Database;
using Sources.Database.DataObject;
using SQLite;
using UnityEngine;
using Enemy = Sources.Database.DataObject.Enemy;

namespace Sources.Unity {
    public class DatabaseManager : MonoBehaviour {
#if UNITY_EDITOR
        [Button("Recreate database")]
        public void SeedDatabase() {
            const string sprites = "Sprites/";

            DatabaseConnector connector = DatabaseConnector.getInstance();

            SQLiteConnection conn = connector.GetConnection();
            RecreateTable<Character>();
            RecreateTable<CardType>();
            RecreateTable<Level>();
            RecreateTable<RowWidth>();
            RecreateTable<Enemy>();
            RecreateTable<Chest>();
            RecreateTable<ItemEffect>();
            RecreateTable<Item>();
            RecreateTable<Chance>();

            const string monsters = sprites + "Monsters/";
            conn.InsertAll(new[] {
                new Character { Name = "Player", Health = 50, Sprite = monsters + "monster (277)" }
            });
            conn.InsertAll(new[] {
                new Level { Name = "Infinite recursion", Length = 5, Width = 3, Difficulty = 1 }
            });
            conn.InsertAll(new[] {
                new CardType { Value = nameof(Character) },
                new CardType { Value = nameof(Enemy) },
                new CardType { Value = nameof(Chest) },
                new CardType { Value = nameof(Item) },
                new CardType { Value = nameof(Level) },
            });
            conn.InsertAll(new[] {
                new RowWidth { Value = 1 },
                new RowWidth { Value = 2 },
                new RowWidth { Value = 3 },
            });
            conn.InsertAll(GetEnemies(monsters));
            conn.InsertAll(GetChests());
            conn.InsertAll(GetItemEffects());
            conn.InsertAll(GetItems(sprites + "Items/"));

            Level testLevel = Level.First(x => x.Name == "Infinite recursion");
            conn.InsertAll(new[] {
                Chance.Make(testLevel, RowWidth.First(x => x.Value == 3), 70),
                Chance.Make(testLevel, RowWidth.First(x => x.Value == 2), 25),
                Chance.Make(testLevel, RowWidth.First(x => x.Value == 1), 5),
                
                Chance.Make(testLevel, testLevel, 100), // cycle
            });
            
            foreach (var item in Item.GetAll()) {
                conn.Insert(Chance.Make(testLevel, item, 100));
            }
            foreach (var enemy in Enemy.GetAll()) {
                conn.Insert(Chance.Make(testLevel, enemy, 100));
            }
            foreach (var chest in Chest.GetAll()) {
                conn.Insert(Chance.Make(testLevel, chest, 100));
                
                foreach (var item in Item.GetAll()) {
                    conn.Insert(Chance.Make(chest, item, 100));
                }
                conn.InsertAll(new[] {
                    Chance.Make(chest, RowWidth.First(x => x.Value == 3), 90),
                    Chance.Make(chest, RowWidth.First(x => x.Value == 2), 10),
                });
            }
        }

        private static IEnumerable<Item> GetItems(string items) {
            return new[] {
                new Item {
                    Name = "Coin", Strongness = Strongness.Easy,
                    Type = ItemType.Equippable,
                    Sprite = null,
                    Count = 1,
                },
                new Item {
                    Name = "Coins", Strongness = Strongness.Easy,
                    Type = ItemType.Equippable,
                    Sprite = null,
                    Count = 3,
                },
                new Item {
                    Name = "Minor health potion", Strongness = Strongness.Easy, Effects = new[] {
                        ItemEffect.First(x => x.Name == "Heal" && x.Strongness == Strongness.Easy),
                        ItemEffect.First(x => x.Name == "Heal" && x.Strongness == Strongness.Hard),
                    },
                    Type = ItemType.Consumable,
                    Sprite = items + "potions:red small",
                    Count = 1,
                },
                new Item {
                    Name = "Health potion", Strongness = Strongness.Hard, Effects = new[] {
                        ItemEffect.First(x => x.Name == "Heal" && x.Strongness == Strongness.Hard)
                    },
                    Type = ItemType.Consumable,
                    Sprite = items + "potions:red large",
                    Count = 1,
                },
            };
        }

        private static IEnumerable<ItemEffect> GetItemEffects() {
            return new[] {
                new ItemEffect { Name = "Heal", Value = 5, Strongness = Strongness.Easy },
                new ItemEffect { Name = "Heal", Value = 10, Strongness = Strongness.Hard },
            };
        }

        private static IEnumerable<Chest> GetChests() {
            return new[] {
                new Chest { Name = "Chest", Length = 1, Width = 3, Strongness = Strongness.Easy },
                new Chest { Name = "Golden Chest", Length = 2, Width = 3, Strongness = Strongness.Hard },
            };
        }

        private static IEnumerable<Enemy> GetEnemies(string monsters) {
            return new[] {
                new Enemy { Name = "Bird", Health = 2, Strongness = Strongness.Easy, Sprite = monsters + "monster (24)" },
                new Enemy { Name = "Ghost", Health = 3, Strongness = Strongness.Easy, Sprite = monsters + "monster (103)" },
                new Enemy { Name = "Training Stand", Health = 1, Strongness = Strongness.Easy },
                new Enemy { Name = "Wolf", Health = 3, Strongness = Strongness.Easy, Sprite = monsters + "monster (49)" },
                new Enemy { Name = "Octopus", Health = 4, Strongness = Strongness.Easy, Sprite = monsters + "monster (141)" },
                new Enemy { Name = "Orc", Health = 5, Strongness = Strongness.Easy, Sprite = monsters + "monster (110)" },

                new Enemy { Name = "Stump", Health = 7, Strongness = Strongness.Hard, Sprite = monsters + "monster (267)" },
                new Enemy { Name = "Skeleton", Health = 6, Strongness = Strongness.Hard, Sprite = monsters + "monster (240)" },
                new Enemy { Name = "Fire Creature", Health = 10, Strongness = Strongness.Hard, Sprite = monsters + "monster (118)" },
                new Enemy { Name = "Cat and Snake", Health = 6, Strongness = Strongness.Hard, Sprite = monsters + "monster (174)" },
                new Enemy { Name = "Lizard", Health = 11, Strongness = Strongness.Hard, Sprite = monsters + "monster (150)" },
                new Enemy { Name = "Mamonth", Health = 15, Strongness = Strongness.Hard, Sprite = monsters + "monster (155)" },
                new Enemy { Name = "Turtle", Health = 9, Strongness = Strongness.Hard, Sprite = monsters + "monster (269)" },

                new Enemy { Name = "Black Knight", Health = 20, Strongness = Strongness.Boss, Sprite = monsters + "monster (277)" },
                new Enemy { Name = "Medusa", Health = 21, Strongness = Strongness.Boss, Sprite = monsters + "monster (255)" },
                new Enemy { Name = "Dragon", Health = 23, Strongness = Strongness.Boss, Sprite = monsters + "monster (9)" },
            };
        }

        private static void RecreateTable<T>() {
            DatabaseConnector connector = DatabaseConnector.getInstance();

            SQLiteConnection conn = connector.GetConnection();
            conn.DropTable<T>();
            conn.CreateTable<T>();
        }
#endif
    }
}
