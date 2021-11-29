using Leopotam.Ecs;
using Sources.Data;
using Sources.Database.DataObject;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.LevelGeneration;

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
        private CardEntityGenerator cardEntityGenerator;
        private Configuration configuration;

        public void Run() {
            if (startFilter.IsEmpty()) return;

            object[][] layout = runtimeData.LevelLayout;
            for (int i = 0; i < layout.Length; i++) {
                for (int j = 0; j < layout[i].Length; j++) {
                    if (layout[i][j] == null) continue;
                    // i - row number (Y), j - position in row - x, so its inverted

                    if (layout[i][j] is Character character && !playerObject.IsEmpty()) {
                        foreach (int idx in playerObject) {
                            playerObject.GetEntity(idx).Replace(new LevelPosition { X = j, Y = i });
                        }
                    } else {
                        cardEntityGenerator.CreateCardEntity(layout[i][j], j, i);
                    }
                }
            }
        }
    }
}
