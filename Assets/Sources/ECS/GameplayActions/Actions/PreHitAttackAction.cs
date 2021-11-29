using Leopotam.Ecs;
using Sources.ECS.GameplayActions.Components;

namespace Sources.ECS.GameplayActions.Actions {
    public class PreHitAttackAction : IGameplayAction {
        public bool ShouldAct(EcsEntity entity) {
            return entity.Has<PreHit>();
        }

        public object[] Act(EcsEntity entity) {
            Hit hit = entity.Get<PreHit>().Hit;
            entity.Del<PreHit>();
            return new object[] { hit };
        }
    }
}
