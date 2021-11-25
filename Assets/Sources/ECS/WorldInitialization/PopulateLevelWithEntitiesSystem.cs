using System.Collections.Generic;
using System.Linq;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Database.DataObject;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Events;
using Sources.LevelGeneration;
using UnityEngine;
using Enemy = Sources.Database.DataObject.Enemy;
using EnemyComponent = Sources.ECS.Components.Gameplay.Enemy;

namespace Sources.ECS.WorldInitialization {
    public class PopulateLevelWithEntitiesSystem : IEcsRunSystem {
        /// <summary>
        /// Creates card entities according to level layout. Also handles player is preserved between levels
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;

        private EcsFilter<StartLevelEvent> startFilter;
        private EcsFilter<Player, Spawned> playerObject;
        private LevelGenerator levelGenerator;
        private Configuration configuration;

        public void Run() {
            if (startFilter.IsEmpty()) return;

            object[][] layout = runtimeData.LevelLayout;
            for (int i = 0; i < layout.Length; i++) {
                for (int j = 0; j < layout[i].Length; j++) {
                    if (layout[i][j] == null) continue;
                    // i - row number (Y), j - position in row - x, so its inverted
                    createCardEntity(layout[i][j], j, i);
                }
            }
        }

        private void createCardEntity(object data, int x, int y) {
            EcsEntity entity;
            LevelPosition position = new() { X = x, Y = y };
            switch (data) {
                case Character character:
                    if (!playerObject.IsEmpty()) {
                        // Player already spawned!
                        foreach (int idx in playerObject) {
                            entity = playerObject.GetEntity(idx);
                            entity.Replace(position);
                        }
                    } else {
                        entity = MakeDefaultCardEntity(position, null, character.Sprite);
                        entity.Replace(new Player { Data = character });
                        entity.Replace(new Hoverable());
                        entity.Replace(new Clickable());
                        entity.Replace(new Draggable());
                        entity.Replace(new Inventory(new Dictionary<Item, int>()));

                        entity.Replace(new Health { Value = character.Health });
                        entity.Replace(new MaxHealth { Value = character.Health });
                    }

                    break;
                case Enemy enemy:
                    entity = MakeDefaultCardEntity(position, null, enemy.Sprite);
                    entity.Replace(new EnemyComponent { Data = enemy });
                    entity.Replace(new Health { Value = enemy.Health });

                    break;
                case Chest chest:
                    entity = MakeDefaultCardEntity(position, chest.Name, chest.Sprite);
                    entity.Replace(new LevelEntrance { Data = chest, Layout = levelGenerator.Generate(chest, runtimeData.CurrentCharacter, new ChestExit()) });

                    break;

                case ChestExit:
                    (ILevelDefinition prevLevel, object[][] prevLevelLayout) = runtimeData.PlayerPath.Peek();
                    entity = MakeDefaultCardEntity(position, "Return to " + prevLevel.Name);
                    entity.Replace(new LevelEntrance { Data = prevLevel, Layout = prevLevelLayout });
                    break;

                case Level level:
                    entity = MakeDefaultCardEntity(position, level.Name, level.Sprite);
                    level.Difficulty *= 1.1f;
                    entity.Replace(new LevelEntrance { Data = level, Layout = levelGenerator.Generate(level, runtimeData.CurrentCharacter) });

                    break;
                case Item item:
                    entity = MakeDefaultCardEntity(position, item.Name, item.Sprite);
                    switch (item.Type) {
                        case ItemType.Consumable:
                            entity.Replace(new ConsumableItem { Data = item });
                            break;
                        case ItemType.Equippable:
                            entity.Replace(new EquippableItem { Data = item });
                            break;
                        default:
                            Debug.LogWarning("Item entity is built, but its not consumable or equippable!");
                            break;
                    }

                    entity.Replace(new CardEffects { Effects = item.Effects.ToList() });
                    //
                    // foreach (ItemEffect effect in item.Effects) {
                    //     // TODO: Add more effects and summarize them if they intersect
                    //     if (effect.Name == "Heal") {
                    //         int healValue = (int)effect.Value * item.Count;
                    //         entity.Replace(new Health { Amount = healValue });
                    //         entity.Replace(new HealthPotion { Amount = healValue });
                    //     } else if (effect.Name == "Resurrection") {
                    //         entity.Replace(new Health { Amount = (int)effect.Value });
                    //         entity.Replace(new Res { Amount = healValue });
                    //     }
                    // }

                    break;
            }
        }

        private EcsEntity MakeDefaultCardEntity(LevelPosition position, string name = null, string spriteDef = null) {
            EcsEntity entity = world.NewEntity();
            entity.Replace(new PlayableCard());
            entity.Replace(position);
            if (!string.IsNullOrEmpty(name)) {
                entity.Replace(new Name { Value = name });
            }

            if (!string.IsNullOrEmpty(spriteDef)) {
                Sprite sprite = default;
                if (spriteDef.Contains(":")) {
                    var splited = spriteDef.Split(":", 2);
                    sprite = Resources.LoadAll<Sprite>(splited[0]).FirstOrDefault(x => x.name == splited[1]);
                } else {
                    sprite = Resources.Load<Sprite>(spriteDef);
                }

                if (sprite != default) {
                    entity.Replace(new Face { Sprite = sprite });
                }
            }

            return entity;
        }
    }
}
