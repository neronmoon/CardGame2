using Sources.Database.DataObject;

namespace Sources.ECS.GameplayActions.Components {
    public struct LevelChange : IShouldDisappear {
        public ILevelDefinition LevelData;
        public object[][] Layout;
    }
}
