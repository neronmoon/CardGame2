using Leopotam.Ecs;

namespace Sources.ECS.Components.Events {
    public struct PlayerMovedEvent {
        public EcsEntity Player;
        public EcsEntity Card;
    }
}
