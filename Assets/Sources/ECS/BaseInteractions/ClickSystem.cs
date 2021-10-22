using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.BaseInteractions.Components;
using Sources.ECS.Components;

namespace Sources.ECS.BaseInteractions {
    public class ClickSystem : IEcsRunSystem {
        /// <summary>
        /// Adds/removes Clicked component
        /// </summary>
        private EcsWorld world;

        private EcsFilter<Hoverable, Hovered, Clickable, VisualObject> clickables;
        private RuntimeData runtimeData;

        public void Run() {
            bool keyDown = runtimeData.Input.Primary;

            foreach (int idx in clickables) {
                EcsEntity entity = clickables.GetEntity(idx);
                bool alreadyClicked = entity.Has<Clicked>();
                if (keyDown && !alreadyClicked) {
                    entity.Replace(new Clicked());
                }

                if (!keyDown && alreadyClicked) {
                    entity.Del<Clicked>();
                }
            }
        }
    }
}
