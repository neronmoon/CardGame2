using Sources.Data.Gameplay;

namespace Sources.ECS.GameplayActions.Components {
    public struct LevelChange : IShouldDisappear {
        public Level Level;
        public object[][] Layout;
    }
}
