using Leopotam.Ecs;

namespace Sources.GameplayActions.Components {
    public struct Hit : IGameplayTrigger {
        public EcsEntity Source;
        public int Amount;
    }
}
