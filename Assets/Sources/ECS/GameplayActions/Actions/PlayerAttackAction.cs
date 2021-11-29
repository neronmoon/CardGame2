using Leopotam.Ecs;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.GameplayActions.Components;
using Enemy = Sources.ECS.Components.Gameplay.CardTypes.Enemy;

namespace Sources.ECS.GameplayActions.Actions {
    public class PlayerAttackAction : IGameplayMoveAction {
        public bool ShouldAct(EcsEntity entity, EcsEntity target) => target.Has<Enemy>() && target.Has<Health>();

        public object[] Act(EcsEntity entity, EcsEntity target) { 
            // target is enemy
            // hit added to player (entity)
            return new object[] { new Hit { Source = target, Amount = target.Get<Health>().Value, ByPlayer = true} };
        }
    }
}
