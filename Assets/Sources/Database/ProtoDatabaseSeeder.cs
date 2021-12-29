using System.Collections.Generic;
using Sources.Data.Gameplay;
using Sources.Database.DataObject;
using SQLite;

namespace Sources.Database {
    public class ProtoDatabaseSeeder {
        public void Seed() {
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
                new Character { Name = "Player", Health = 50, Sprite = monsters + "monster (277)", Description = "This is you." }
            });
            Level level = new Level {
                Name = "Infinite recursion",
                Length = 15,
                Width = 3,
                Difficulty = 1,
                Sprite = sprites + "Portal",
                SubLevelCount = 3,
                Description = "Infinite level, that you cannot escape!"
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

                Chance.Make(bag, Item.ByName("Coin"), 25),
                Chance.Make(bag, Item.ByName("Coins"), 25),
                Chance.Make(bag, Item.ByName("Minor health potion"), 20),
                Chance.Make(bag, Item.ByName("Health potion"), 10),
                Chance.Make(bag, Item.ByName("Resurrection stone"), 10),
                Chance.Make(bag, Item.ByName("Map"), 10),
            });

            Chest chest = Chest.ByName("Chest");
            conn.InsertAll(new[] {
                Chance.Make(chest, RowWidth.First(x => x.Value == 3), 90),
                Chance.Make(chest, RowWidth.First(x => x.Value == 2), 10),

                Chance.Make(chest, Item.ByName("Coin"), 25),
                Chance.Make(chest, Item.ByName("Coins"), 25),
                Chance.Make(chest, Item.ByName("Minor health potion"), 20),
                Chance.Make(chest, Item.ByName("Health potion"), 10),
                Chance.Make(chest, Item.ByName("Map"), 10),
                Chance.Make(chest, Item.ByName("Resurrection stone"), 10),
            });

            foreach (Enemy enemy in Enemy.GetAll()) {
                conn.InsertAll(new[] {
                    Chance.Make(enemy, Item.ByName("Coin"), 30),
                    Chance.Make(enemy, Item.ByName("Coins"), 20),
                    Chance.Make(enemy, Item.ByName("Minor health potion"), 50),
                });
            }
        }

        private static IEnumerable<Item> GetItems(string items) {
            return new[] {
                new Item {
                    Name = "Coin",
                    Strongness = Strongness.Easy,
                    Type = ItemType.Equippable,
                    Sprite = items + "Misc/Golden Coin",
                    Count = 1,
                    Description = "Just gold coin. Spend it wisely"
                },
                new Item {
                    Name = "Coins",
                    Strongness = Strongness.Easy,
                    Type = ItemType.Equippable,
                    Sprite = items + "Misc/Golden Coins",
                    Count = 3,
                    Description = "Bag of coins"
                },
                new Item {
                    Name = "Map",
                    Strongness = Strongness.Easy,
                    Type = ItemType.Equippable,
                    Sprite = items + "Misc/Map",
                    Count = 1,
                    ShowInInventory = true,
                    Description = "You know, a map. Shows you da way"
                },
                new Item {
                    Name = "Resurrection stone",
                    Strongness = Strongness.Hard,
                    Type = ItemType.Equippable,
                    Effects = new[] {
                        ItemEffect.First(e => e.Name == ItemEffectType.Resurrection && e.Strongness == Strongness.Hard)
                    },
                    Sprite = items + "Misc/Rune Stone",
                    Count = 1,
                    ShowInInventory = true,
                    Description = "Prevents you from death"
                },
                new Item {
                    Name = "Minor health potion",
                    Strongness = Strongness.Easy,
                    Effects = new[] {
                        ItemEffect.First(x => x.Name == ItemEffectType.Heal && x.Strongness == Strongness.Easy),
                    },
                    Type = ItemType.Consumable,
                    Sprite = items + "Potion/Red Potion",
                    Count = 1,
                    Description = "Delicious orange juice that heals you"
                },
                new Item {
                    Name = "Health potion",
                    Strongness = Strongness.Hard,
                    Effects = new[] {
                        ItemEffect.First(x => x.Name == ItemEffectType.Heal && x.Strongness == Strongness.Hard)
                    },
                    Type = ItemType.Consumable,
                    Sprite = items + "Potion/Red Potion 2",
                    Count = 1,
                    Description = "One fucking liter of health potion. So strong, that makes you cry"
                },
            };
        }

        private static IEnumerable<ItemEffect> GetItemEffects() {
            return new[] {
                new ItemEffect { Name = ItemEffectType.Heal, Value = 5, Strongness = Strongness.Easy },
                new ItemEffect { Name = ItemEffectType.Heal, Value = 10, Strongness = Strongness.Hard },
                new ItemEffect { Name = ItemEffectType.Resurrection, Value = 5, Strongness = Strongness.Hard },
            };
        }

        private static IEnumerable<Chest> GetChests(string items) {
            return new[] {
                new Chest {
                    Name = "Small bag",
                    Length = 1,
                    Width = 3,
                    Sprite = items + "Equipment/Bag",
                    Strongness = Strongness.Easy,
                    Description = "Forgotten bag. May contain some gold or helpful stuff"
                },
                new Chest {
                    Name = "Chest",
                    Length = 2,
                    Width = 3,
                    Sprite = items + "Misc/Chest",
                    Strongness = Strongness.Hard,
                    Description = "Just chest"
                },
            };
        }

        private static IEnumerable<Enemy> GetEnemies(string monsters) {
            return new[] {
                new Enemy {
                    Name = "Bird",
                    Health = 2,
                    Strongness = Strongness.Easy,
                    Sprite = monsters + "monster (24)",
                    IsAggressive = true,
                    Description = "Bird, that makes you cry"
                },
                new Enemy {
                    Name = "Ghost",
                    Health = 3,
                    Strongness = Strongness.Easy,
                    Sprite = monsters + "monster (103)",
                    IsAggressive = true,
                    Description = "This is ghost of Jared Bollor that used to be fucking aggressive abuser. Kill it now!"
                },
                new Enemy {
                    Name = "Training Stand",
                    Health = 1,
                    Strongness = Strongness.Easy,
                    Description = "Wooden thing that you use to practice"
                },
                new Enemy {
                    Name = "Wolf",
                    Health = 3,
                    Strongness = Strongness.Easy,
                    Sprite = monsters + "monster (49)",
                    IsAggressive = true,
                    Description = "Woooof! Skinny angry wolf. Must be hungry"
                },
                new Enemy {
                    Name = "Octopus",
                    Health = 4,
                    Strongness = Strongness.Easy,
                    Sprite = monsters + "monster (141)",
                    IsAggressive = true,
                    Description = "Wow! Octopus! Never thought they can live in recursion levels. mAgIc!!1"
                },
                new Enemy {
                    Name = "Orc",
                    Health = 5,
                    Strongness = Strongness.Easy,
                    Sprite = monsters + "monster (110)",
                    IsAggressive = true,
                    Description = "Just simple orc. A small one."
                },

                new Enemy {
                    Name = "Stump",
                    Health = 7,
                    Strongness = Strongness.Hard,
                    Sprite = monsters + "monster (267)",
                    IsAggressive = true,
                    Description = "Dead tree, that want you to be dead too"
                },
                new Enemy {
                    Name = "Skeleton",
                    Health = 6,
                    Strongness = Strongness.Hard,
                    Sprite = monsters + "monster (240)",
                    IsAggressive = true,
                    Description = "Bunch of calcium, its good for you teeth"
                },
                new Enemy {
                    Name = "Fire Creature",
                    Health = 9,
                    Strongness = Strongness.Hard,
                    Sprite = monsters + "monster (118)",
                    IsAggressive = true,
                    Description = "It's warm. Too warm"
                },
                new Enemy {
                    Name = "Cat and Snake",
                    Health = 6,
                    Strongness = Strongness.Hard,
                    Sprite = monsters + "monster (174)",
                    IsAggressive = true,
                    Description = "Strange couple. Did you know that cats should be afraid of snakes? So this one does not"
                },
                new Enemy {
                    Name = "Lizard",
                    Health = 10,
                    Strongness = Strongness.Hard,
                    Sprite = monsters + "monster (150)",
                    IsAggressive = true,
                    Description = "A lizard"
                },
                new Enemy {
                    Name = "Mamonth",
                    Health = 11,
                    Strongness = Strongness.Hard,
                    Sprite = monsters + "monster (155)",
                    IsAggressive = true,
                    Description = "A big elephant. Thought they are all dead"
                },
                new Enemy {
                    Name = "Turtle",
                    Health = 8,
                    Strongness = Strongness.Hard,
                    Sprite = monsters + "monster (269)",
                    IsAggressive = true, 
                    Description = "Not nice turtle"
                },

                new Enemy {
                    Name = "Black Knight",
                    Health = 12,
                    Strongness = Strongness.Boss,
                    Sprite = monsters + "monster (277)",
                    IsAggressive = true,
                    Description = "This guy looks scary!"
                },
                new Enemy {
                    Name = "Medusa",
                    Health = 13,
                    Strongness = Strongness.Boss,
                    Sprite = monsters + "monster (255)",
                    IsAggressive = true,
                    Description = "Bunch of snakes and a skull"
                },
                new Enemy {
                    Name = "Dragon",
                    Health = 14,
                    Strongness = Strongness.Boss,
                    Sprite = monsters + "monster (9)",
                    IsAggressive = true,
                    Description = "Elder dragon that loves money! Euros mostly"
                },
            };
        }

        private static void RecreateTable<T>() {
            DatabaseConnector connector = DatabaseConnector.getInstance();

            SQLiteConnection conn = connector.GetConnection();
            conn.DropTable<T>();
            conn.CreateTable<T>();
        }
    }
}
