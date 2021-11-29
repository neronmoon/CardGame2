using Leopotam.Ecs;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.GameplayActions.Components;

namespace Sources.ECS.GameplayActions.Actions {
    public class EnterLevelAction : IGameplayMoveAction {
        public bool ShouldAct(EcsEntity entity, EcsEntity target) => target.Has<LevelEntrance>();

        public object[] Act(EcsEntity entity, EcsEntity target) {
            LevelEntrance entrance = target.Get<LevelEntrance>();
            return new object[] { new LevelChange { LevelData = entrance.Data, Layout = entrance.Layout } };
        }
    }
}
