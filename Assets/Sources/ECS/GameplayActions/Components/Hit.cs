using Leopotam.Ecs;

namespace Sources.ECS.GameplayActions.Components {
    public struct Hit : IGameplayTrigger {
        public EcsEntity Source;
        public int Amount;
    }
}
