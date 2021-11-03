using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Events;
using EnemySpec = Sources.Data.Gameplay.Enemy;
using Enemy = Sources.ECS.Components.Gameplay.Enemy;
using HealthPotion = Sources.ECS.Components.Gameplay.HealthPotion;
using HealthPotionSpec = Sources.Data.Gameplay.HealthPotion;

namespace Sources.ECS.WorldInitialization {
    public class PopulateLevelWithEntitiesSystem : IEcsRunSystem {
        /// <summary>
        /// Creates card entitites according to level layout. Also handles player is preserved between levels
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

                        entity.Replace(new Health { Amount = character.Health });

                        if (character.Sprite) {
                            entity.Replace(new Face { Sprite = character.Sprite });
                        }
                    }

                    break;
                case EnemySpec enemy:
                    entity = MakeDefaultCardEntity(position);
                    entity.Replace(new Enemy { Data = enemy });
                    entity.Replace(new Health { Amount = enemy.Health });
                    if (enemy.Sprite) {
                        entity.Replace(new Face { Sprite = enemy.Sprite });
                    }

                    break;
                case Chest chest:
                    entity = MakeDefaultCardEntity(position);

                    // define chest level
                    // this should exit to current level at chest position 
                    Level chestLevel = chest.GetLevel();
                    entity.Replace(new LevelExit {
                        Data = chestLevel,
                        Layout = levelGenerator.Generate(chestLevel, configuration.Character, true) // TODO: replace true with some other value 
                    });

                    if (!string.IsNullOrEmpty(chest.Name)) {
                        entity.Replace(new Name { Value = chest.Name });
                    }

                    if (chest.Sprite) {
                        entity.Replace(new Face { Sprite = chest.Sprite });
                    }

                    break;

                // this is chest exit
                case true:
                    entity = MakeDefaultCardEntity(position);
                    (Level prevLevel, object[][] prevLevelLayout) = runtimeData.PlayerPath.Peek();
                    entity.Replace(new LevelExit {
                        Data = prevLevel,
                        Layout = prevLevelLayout
                    });

                    entity.Replace(new Name { Value = "Return to " + prevLevel.Name });
                    break;

                case Level level:
                    entity = MakeDefaultCardEntity(position);
                    entity.Replace(new LevelExit { Data = level });
                    if (!string.IsNullOrEmpty(level.Name)) {
                        entity.Replace(new Name { Value = level.Name });
                    }

                    if (level.Sprite) {
                        entity.Replace(new Face { Sprite = level.Sprite });
                    }

                    break;
                case Item item:
                    entity = MakeDefaultCardEntity(position);

                    if (!string.IsNullOrEmpty(item.Name)) {
                        entity.Replace(new Name { Value = item.Name });
                    }

                    if (item.Sprite) {
                        entity.Replace(new Face { Sprite = item.Sprite });
                    }

                    if (item is HealthPotionSpec potion) {
                        entity.Replace(new Health { Amount = potion.Amount });
                        entity.Replace(new HealthPotion { Amount = potion.Amount });
                    }

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
