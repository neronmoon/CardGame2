using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;

namespace Sources.ECS.Movement {
    public class UpdateLevelLayoutOnPlayerMoveSystem : IEcsRunSystem {
        /// <summary>
        /// Adjusts current level layout in runtime data.
        /// If player is going into chest we need to place him back when he returns
        /// </summary>
        private EcsWorld world;

        private RuntimeData runtimeData;
        private Configuration configuration;
        private EcsFilter<PlayableCard, LevelPosition>.Exclude<Leftover> cards;

        public void Run() {
            foreach (int idx in cards) {
                LevelPosition pos = cards.Get2(idx);
                EcsEntity entity = cards.GetEntity(idx);

                if (entity.Has<Discarded>()) {
                    runtimeData.LevelLayout[pos.Y][pos.X] = null;
                } else if (entity.Has<Player>()) {
                    runtimeData.LevelLayout[pos.Y][pos.X] = runtimeData.CurrentCharacter;
                }
                
            }
        }
    }
}
