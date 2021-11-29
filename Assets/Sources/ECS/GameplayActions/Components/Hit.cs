using Leopotam.Ecs;

namespace Sources.ECS.GameplayActions.Components {
    public struct Hit : IShouldDisappear {
        public EcsEntity Source;
        public int Amount;
        public bool ByPlayer;
    }
}
