using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using EnemySpec = Sources.Data.Enemy;
using Enemy = Sources.ECS.Components.Enemy;

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
                    createCardEntity(layout[i][j], i, j);
                }
            }
        }

        private void createCardEntity(object data, int x, int y) {
            EcsEntity entity = world.NewEntity();
            entity.Replace(new PlayableCard());
            entity.Replace(new LevelPosition { X = x, Y = y });
            switch (data) {
                case Character character:
                    entity.Replace(new Player());
                    entity.Replace(new Hoverable());
                    entity.Replace(new Clickable());
                    break;
                case EnemySpec enemy:
                    entity.Replace(new Enemy());
                    break;
                case Level level:
                    entity.Replace(new LevelExit());
                    break;
            }
        }
    }
}
