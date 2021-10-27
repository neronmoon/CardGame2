using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Events;
using EnemySpec = Sources.Data.Gameplay.Enemy;
using Enemy = Sources.ECS.Components.Gameplay.Enemy;

namespace Sources.ECS.WorldInitialization {
    public class CreateLevelEntitiesSystem : IEcsRunSystem {
        /// <summary>
        /// Spawn cards according to level layout from runtime data
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;

        private EcsFilter<StartLevelEvent> startFilter;

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
            EcsEntity entity = world.NewEntity();
            entity.Replace(new PlayableCard());
            entity.Replace(new LevelPosition { X = x, Y = y });
            switch (data) {
                case Character character:
                    entity.Replace(new Player { Data = character });
                    entity.Replace(new Hoverable());
                    entity.Replace(new Clickable());
                    entity.Replace(new Draggable());

                    entity.Replace(new Health { Amount = character.Health });

                    if (character.Sprite) {
                        entity.Replace(new Face { Sprite = character.Sprite });
                    }

                    break;
                case EnemySpec enemy:
                    entity.Replace(new Enemy { Data = enemy });
                    entity.Replace(new Health { Amount = enemy.Health });
                    if (enemy.Sprite) {
                        entity.Replace(new Face { Sprite = enemy.Sprite });
                    }

                    break;
                case Level level:
                    entity.Replace(new LevelExit { Data = level });
                    if (level.Sprite) {
                        entity.Replace(new Face { Sprite = level.Sprite });
                    }

                    if (!string.IsNullOrEmpty(level.Name)) {
                        entity.Replace(new Name { Value = level.Name });
                    }

                    break;
            }
        }
    }
}
