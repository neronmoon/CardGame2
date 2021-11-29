using Leopotam.Ecs;

namespace Sources.ECS.GameplayActions.Actions {
    public interface IGameplayAction {
        public abstract bool ShouldAct(EcsEntity entity);
        public abstract object[] Act(EcsEntity entity);
    }
    
    public interface IGameplayMoveAction {
        public abstract bool ShouldAct(EcsEntity entity, EcsEntity target);
        public abstract object[] Act(EcsEntity entity, EcsEntity target);
    }
}
