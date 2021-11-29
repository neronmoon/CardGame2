using Leopotam.Ecs;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;

namespace Sources.ECS.GameplayActions.Actions {
    public class DieAction : IGameplayAction {
        public bool ShouldAct(EcsEntity entity) => entity.Has<Health>() && entity.Get<Health>().Value <= 0 && !entity.Has<Dead>();

        public object[] Act(EcsEntity entity) {
            return new object[] { new Dead() };
        }
    }
}
