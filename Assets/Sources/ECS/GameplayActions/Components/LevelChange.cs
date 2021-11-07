using Sources.Data.Gameplay;

namespace Sources.ECS.GameplayActions.Components {
    public struct LevelChange : IShouldDisappear {
        public LevelData LevelData;
        public object[][] Layout;
    }
}
