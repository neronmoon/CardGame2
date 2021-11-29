using System.Collections;
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
                new CardType { Value = CardType.Character },
                new CardType { Value = CardType.Enemy },
                new CardType { Value = CardType.Chest },
                new CardType { Value = CardType.Item },
                new CardType { Value = CardType.Level },
                new CardType { Value = CardType.NPC },
            });
            conn.InsertAll(new[] {
                new RowWidth { Value = 1 },
                new RowWidth { Value = 2 },
                new RowWidth { Value = 3 },
            });
            conn.InsertAll(GetEnemies(monsters));
            conn.InsertAll(GetChests(sprites + "Items/"));
            conn.InsertAll(GetItemEffects());
            conn.InsertAll(GetItems(sprites + "Items/"));

            conn.InsertAll(new[] {
                new Character { Name = "Player", Health = 50, Sprite = monsters + "monster (277)" }
            });
            Level level = new Level {
                Name = "Infinite recursion",
                Length = 15,
                Width = 3,
                Difficulty = 1,
                Sprite = sprites + "Portal",
                SubLevelCount = 3
            };
            conn.InsertAll(new[] { level });

            conn.InsertAll(new[] {
                Chance.Make(level, RowWidth.First(x => x.Value == 3), 70),
                Chance.Make(level, RowWidth.First(x => x.Value == 2), 25),
                Chance.Make(level, RowWidth.First(x => x.Value == 1), 5),

                Chance.Make(level, level, 100), // cycle

                // easy
                Chance.Make(level, Enemy.ByName("Bird"), 20),
                Chance.Make(level, Enemy.ByName("Ghost"), 20),
                Chance.Make(level, Enemy.ByName("Wolf"), 20),
                Chance.Make(level, Enemy.ByName("Octopus"), 20),
                Chance.Make(level, Enemy.ByName("Orc"), 20),

                // hard
                Chance.Make(level, Enemy.ByName("Stump"), 14),
                Chance.Make(level, Enemy.ByName("Skeleton"), 14),
                Chance.Make(level, Enemy.ByName("Fire Creature"), 14),
                Chance.Make(level, Enemy.ByName("Cat and Snake"), 14),
                Chance.Make(level, Enemy.ByName("Lizard"), 14),
                Chance.Make(level, Enemy.ByName("Mamonth"), 14),
                Chance.Make(level, Enemy.ByName("Turtle"), 14),

                // boss
                Chance.Make(level, Enemy.ByName("Black Knight"), 33),
                Chance.Make(level, Enemy.ByName("Medusa"), 33),
                Chance.Make(level, Enemy.ByName("Dragon"), 33),

                Chance.Make(level, Item.ByName("Coin"), 45),
                Chance.Make(level, Item.ByName("Coins"), 15),
                Chance.Make(level, Item.ByName("Minor health potion"), 30),
                Chance.Make(level, Item.ByName("Health potion"), 10),

                Chance.Make(level, Chest.ByName("Small bag"), 70),
                Chance.Make(level, Chest.ByName("Chest"), 30),
            });

            Chest bag = Chest.ByName("Small bag");
            conn.InsertAll(new[] {
                Chance.Make(bag, RowWidth.First(x => x.Value == 3), 90),
                Chance.Make(bag, RowWidth.First(x => x.Value == 2), 10),

                Chance.Make(bag, Item.ByName("Resurrection stone"), 100),
                Chance.Make(bag, Item.ByName("Coin"), 45),
                Chance.Make(bag, Item.ByName("Coins"), 15),
                Chance.Make(bag, Item.ByName("Minor health potion"), 20),
                Chance.Make(bag, Item.ByName("Health potion"), 10),
            });

            Chest chest = Chest.ByName("Chest");
            conn.InsertAll(new[] {
                Chance.Make(chest, RowWidth.First(x => x.Value == 3), 90),
                Chance.Make(chest, RowWidth.First(x => x.Value == 2), 10),

                Chance.Make(chest, Item.ByName("Resurrection stone"), 100),
                Chance.Make(chest, Item.ByName("Coin"), 45),
                Chance.Make(chest, Item.ByName("Coins"), 15),
                Chance.Make(chest, Item.ByName("Minor health potion"), 20),
                Chance.Make(chest, Item.ByName("Health potion"), 10),
            });
        }

        private static IEnumerable<Item> GetItems(string items) {
            return new[] {
                new Item {
                    Name = "Coin",
                    Strongness = Strongness.Easy,
                    Type = ItemType.Equippable,
                    Sprite = items + "Misc/Golden Coin",
                    Count = 1,
                },
                new Item {
                    Name = "Coins",
                    Strongness = Strongness.Easy,
                    Type = ItemType.Equippable,
                    Sprite = items + "Misc/Golden Coins",
                    Count = 3,
                },
                new Item {
                    Name = "Map",
                    Strongness = Strongness.Easy,
                    Type = ItemType.Equippable,
                    Sprite = items + "Misc/Map",
                    Count = 1,
                    ShowInInventory = true,
                },
                new Item {
                    Name = "Resurrection stone",
                    Strongness = Strongness.Hard,
                    Type = ItemType.Equippable,
                    Effects = new[] {
                        ItemEffect.First(e => e.Name == "Resurrection" && e.Strongness == Strongness.Hard)
                    },
                    Sprite = items + "Misc/Rune Stone",
                    Count = 1,
                    ShowInInventory = true,
                },
                new Item {
                    Name = "Minor health potion",
                    Strongness = Strongness.Easy,
                    Effects = new[] {
                        ItemEffect.First(x => x.Name == "Heal" && x.Strongness == Strongness.Easy),
                    },
                    Type = ItemType.Consumable,
                    Sprite = items + "Potion/Red Potion",
                    Count = 1,
                },
                new Item {
                    Name = "Health potion",
                    Strongness = Strongness.Hard,
                    Effects = new[] {
                        ItemEffect.First(x => x.Name == "Heal" && x.Strongness == Strongness.Hard)
                    },
                    Type = ItemType.Consumable,
                    Sprite = items + "Potion/Red Potion 2",
                    Count = 1,
                },
            };
        }

        private static IEnumerable<ItemEffect> GetItemEffects() {
            return new[] {
                new ItemEffect { Name = "Heal", Value = 5, Strongness = Strongness.Easy },
                new ItemEffect { Name = "Heal", Value = 10, Strongness = Strongness.Hard },
                new ItemEffect { Name = "Resurrection", Value = 5, Strongness = Strongness.Hard },
            };
        }

        private static IEnumerable<Chest> GetChests(string items) {
            return new[] {
                new Chest { Name = "Small bag", Length = 1, Width = 3, Sprite = items + "Equipment/Bag", Strongness = Strongness.Easy },
                new Chest { Name = "Chest", Length = 2, Width = 3, Sprite = items + "Misc/Chest", Strongness = Strongness.Hard },
            };
        }

        private static IEnumerable<Enemy> GetEnemies(string monsters) {
            return new[] {
                new Enemy { Name = "Bird", Health = 2, Strongness = Strongness.Easy, Sprite = monsters + "monster (24)", IsAggressive = true },
                new Enemy { Name = "Ghost", Health = 3, Strongness = Strongness.Easy, Sprite = monsters + "monster (103)", IsAggressive = true },
                new Enemy { Name = "Training Stand", Health = 1, Strongness = Strongness.Easy },
                new Enemy { Name = "Wolf", Health = 3, Strongness = Strongness.Easy, Sprite = monsters + "monster (49)", IsAggressive = true },
                new Enemy { Name = "Octopus", Health = 4, Strongness = Strongness.Easy, Sprite = monsters + "monster (141)", IsAggressive = true },
                new Enemy { Name = "Orc", Health = 5, Strongness = Strongness.Easy, Sprite = monsters + "monster (110)", IsAggressive = true },

                new Enemy { Name = "Stump", Health = 7, Strongness = Strongness.Hard, Sprite = monsters + "monster (267)", IsAggressive = true },
                new Enemy { Name = "Skeleton", Health = 6, Strongness = Strongness.Hard, Sprite = monsters + "monster (240)", IsAggressive = true },
                new Enemy { Name = "Fire Creature", Health = 9, Strongness = Strongness.Hard, Sprite = monsters + "monster (118)", IsAggressive = true },
                new Enemy { Name = "Cat and Snake", Health = 6, Strongness = Strongness.Hard, Sprite = monsters + "monster (174)", IsAggressive = true },
                new Enemy { Name = "Lizard", Health = 10, Strongness = Strongness.Hard, Sprite = monsters + "monster (150)", IsAggressive = true },
                new Enemy { Name = "Mamonth", Health = 11, Strongness = Strongness.Hard, Sprite = monsters + "monster (155)", IsAggressive = true },
                new Enemy { Name = "Turtle", Health = 8, Strongness = Strongness.Hard, Sprite = monsters + "monster (269)", IsAggressive = true },

                new Enemy { Name = "Black Knight", Health = 12, Strongness = Strongness.Boss, Sprite = monsters + "monster (277)", IsAggressive = true },
                new Enemy { Name = "Medusa", Health = 13, Strongness = Strongness.Boss, Sprite = monsters + "monster (255)", IsAggressive = true },
                new Enemy { Name = "Dragon", Health = 14, Strongness = Strongness.Boss, Sprite = monsters + "monster (9)", IsAggressive = true },
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
