using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Database.DataObject;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Events;
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
                        entity = MakeDefaultCardEntity(position);
                        entity.Replace(new Player { Data = character });
                        entity.Replace(new Hoverable());
                        entity.Replace(new Clickable());
                        entity.Replace(new Draggable());
                        entity.Replace(new Inventory(new Dictionary<Item, int>()));

                        entity.Replace(new Health { Amount = character.Health });

                        // if (character.Sprite) {
                            // entity.Replace(new Face { Sprite = character.Sprite });
                        // }
                    }

                    break;
                case Enemy enemy:
                    entity = MakeDefaultCardEntity(position);
                    entity.Replace(new EnemyComponent { Data = enemy });
                    entity.Replace(new Health { Amount = enemy.Health });
                    // if (enemy.Sprite) {
                        // entity.Replace(new Face { Sprite = enemy.Sprite });
                    // }

                    break;
                case Chest chest:
                    entity = MakeDefaultCardEntity(position);

                    // define chest level
                    // this should exit to current level at chest position 
                    entity.Replace(new LevelEntrance {
                        Data = chest,
                        Layout = levelGenerator.Generate(chest, runtimeData.CurrentCharacter, new ChestExit())
                    });

                    if (!string.IsNullOrEmpty(chest.Name)) {
                        entity.Replace(new Name { Value = chest.Name });
                    }

                    // if (chest.Sprite) {
                        // entity.Replace(new Face { Sprite = chest.Sprite });
                    // }

                    break;

                case ChestExit:
                    entity = MakeDefaultCardEntity(position);
                    (ILevelDefinition prevLevel, object[][] prevLevelLayout) = runtimeData.PlayerPath.Peek();
                    entity.Replace(new LevelEntrance { Data = prevLevel, Layout = prevLevelLayout });

                    entity.Replace(new Name { Value = "Return to " + prevLevel.Name });
                    break;

                case Level level:
                    entity = MakeDefaultCardEntity(position);
                    entity.Replace(new LevelEntrance {
                        Data = level,
                        Layout = levelGenerator.Generate(level, runtimeData.CurrentCharacter)
                    });
                    if (!string.IsNullOrEmpty(level.Name)) {
                        entity.Replace(new Name { Value = level.Name });
                    }

                    // if (level.Sprite) {
                        // entity.Replace(new Face { Sprite = level.Sprite });
                    // }

                    break;
                case Item item:
                    entity = MakeDefaultCardEntity(position);
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

                    if (!string.IsNullOrEmpty(item.Name)) {
                        entity.Replace(new Name { Value = item.Name });
                    }

                    // if (item.Sprite) {
                        // entity.Replace(new Face { Sprite = item.Sprite });
                    // }

                    // if (item is HealthPotionData potion) {
                        // entity.Replace(new Health { Amount = potion.Amount });
                        // entity.Replace(new HealthPotion { Amount = potion.Amount });
                    // }

                    break;
            }
        }

        private EcsEntity MakeDefaultCardEntity(LevelPosition? position = null) {
            EcsEntity entity = world.NewEntity();
            entity.Replace(new PlayableCard());
            if (position != null) {
                entity.Replace((LevelPosition)position);
            }

            return entity;
        }
    }
}
