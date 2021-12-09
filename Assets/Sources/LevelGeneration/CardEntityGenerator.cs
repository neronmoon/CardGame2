using System.Collections.Generic;
using System.Linq;
using Leopotam.Ecs;
using Sources.Database.DataObject;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.Components.Gameplay.Perks;
using UnityEngine;
using Sources.Data;
using EnemyComponent = Sources.ECS.Components.Gameplay.CardTypes.Enemy;
using Enemy = Sources.Database.DataObject.Enemy;

namespace Sources.LevelGeneration {
    public class CardEntityGenerator {
        private EcsWorld world;
        private RuntimeData runtimeData;
        private LevelGenerator levelGenerator;

        public CardEntityGenerator(EcsWorld world, RuntimeData runtimeData, LevelGenerator levelGenerator) {
            this.world = world;
            this.runtimeData = runtimeData;
            this.levelGenerator = levelGenerator;
        }
        

        public EcsEntity CreateCardEntity(object data, int x, int y) {
            EcsEntity entity = default;
            LevelPosition position = new() { X = x, Y = y };
            switch (data) {
                case Character character:
                    entity = MakeDefaultCardEntity(position, null, character.Sprite);
                    entity.Replace(new Player { Data = character });
                    entity.Replace(new Draggable());
                    entity.Replace(new Inventory {Items = new List<Item>(10)});

                    entity.Replace(new Health { Value = character.Health });
                    entity.Replace(new MaxHealth { Value = character.Health });
                    break;
                case Enemy enemy:
                    entity = MakeDefaultCardEntity(position, null, enemy.Sprite);
                    entity.Replace(new EnemyComponent { Data = enemy });
                    entity.Replace(new Health { Value = enemy.Health });

                    if (enemy.IsAggressive) {
                        entity.Replace(new Aggressive());
                    }

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
                    break;
            }

            return entity;
        }

        private EcsEntity MakeDefaultCardEntity(LevelPosition position, string name = null, string spriteDef = null) {
            EcsEntity entity = world.NewEntity();
            entity.Replace(new PlayableCard());
            entity.Replace(new Hoverable());
            entity.Replace(new Clickable());
            entity.Replace(new DoubleClickable());
            entity.Replace(position);
            if (!string.IsNullOrEmpty(name)) {
                entity.Replace(new Name { Value = name });
            }

            if (!string.IsNullOrEmpty(spriteDef)) {
                Sprite sprite;
                if (spriteDef.Contains(":")) {
                    string[] split = spriteDef.Split(":", 2);
                    sprite = Resources.LoadAll<Sprite>(split[0]).FirstOrDefault(x => x.name == split[1]);
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
