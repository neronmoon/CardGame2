using Leopotam.Ecs;
using Sources.Data;
using Sources.Data.Gameplay;
using Sources.ECS.Components.Events;
using UnityEngine;
using Random = System.Random;

namespace Sources.ECS.WorldInitialization {
    public class GenerateLevelLayoutSystem : IEcsRunSystem {
        /// <summary>
        /// Generates level layout using runtimeData's CurrentLevel prop
        /// </summary>
        private Random random = new();

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
                layout[i] = new object[level.Width];
            }

            // placing player (always in middle of row)
            int center = Mathf.FloorToInt(level.Width / 2);
            layout[0][center] = configuration.Character;

            // generating level
            for (int i = 1; i < layout.Length - 1; i++) {
                int nullsCount = 0;
                for (int j = 0; j < layout[i].Length; j++) {
                    layout[i][j] = choose(new[] { choose(level.Enemies), choose(level.Items) });
                    if (layout[i][j] == null) {
                        nullsCount++;
                    }
                }
            }

            // placing exit at last row
            layout[layout.Length - 1][center] = choose(level.Exits);
            runtimeData.LevelLayout = layout;
        }

        private object choose(object[] objects) {
            return objects[random.Next(0, objects.Length)];
        }
    }
}
