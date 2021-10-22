using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components.Events;
using UnityEngine;
using Random = System.Random;

namespace Sources.ECS.WorldInitialization {
    public class GenerateLevelLayoutSystem : IEcsRunSystem {
        /// <summary>
        /// Generates level layout using runtimeData's CurrentLevel prop
        /// </summary>
        private const int Width = 3;

        private Random random = new Random();

        private EcsWorld world;
        private EcsFilter<StartLevelEvent> filter;
        private RuntimeData runtimeData;
        private Configuration configuration;

        public void Run() {
            if (filter.IsEmpty()) return;

            // layout is matrix (N+2)xM where N is level length and M is width of level
            // fist line is for player, last line for exit
            // layout contains object of player/enemies/exits
            // null means empty space without card
            Level level = runtimeData.CurrentLevel;
            object[][] layout = new object[level.Length + 2][];
            for (int i = 0; i < layout.Length; i++) {
                layout[i] = new object[Width];
            }

            layout[0][1] = configuration.Character; // player always in middle
            for (int i = 1; i < level.Length; i++) {
                int nullsCount = 0;
                for (int j = 0; j < Width; j++) {
                    object enemy = choose(level.Enemies);
                    object exit = choose(level.Exits);
                    object[] options = nullsCount < 1 ? new[] { null, enemy, exit } : new[] { enemy, exit };
                    layout[i][j] = choose(options);
                    if (layout[i][j] == null) {
                        nullsCount++;
                    }
                }
            }

            layout[level.Length - 1][(int)choose(new object[] { 0, 1, 2 })] = choose(level.Exits);
            runtimeData.LevelLayout = layout;
        }

        private object choose(object[] objects) {
            return objects[random.Next(0, objects.Length)];
        }
    }
}
