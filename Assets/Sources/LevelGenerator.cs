using Sources.Data.Gameplay;
using Sources.ECS.Extensions;
using UnityEngine;

namespace Sources {
    public class LevelGenerator {

        public object[][] Generate(Level levelSpec, Character character, object exit = null) {
            // layout is matrix (N+2)xM where N is level length and M is width of level
            // fist line is for player, last line for exit
            // layout contains object of player/enemies/exits
            // null means empty space without card
            object[][] layout = new object[levelSpec.Length + 2][];
            for (int i = 0; i < layout.Length; i++) {
                layout[i] = new object[levelSpec.Width];
            }

            // placing player (always in middle of row)
            int center = Mathf.FloorToInt(levelSpec.Width / 2);
            layout[0][center] = character;

            // generating level
            for (int i = 1; i < layout.Length - 1; i++) {
                int nullsCount = 0;
                for (int j = 0; j < layout[i].Length; j++) {
                    layout[i][j] = new[] { levelSpec.Enemies.ChooseOne(), levelSpec.Items.ChooseOne(), levelSpec.Chests.ChooseOne() }.ChooseOne();
                    if (layout[i][j] == null) {
                        nullsCount++;
                    }
                }
            }

            // placing exit at last row
            layout[^1][center] = exit ?? levelSpec.Exits.ChooseOne();
            return layout;
        }
    }
}
