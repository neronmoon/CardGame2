using Leopotam.Ecs;
using Sources.Data;
using UnityEngine;
using Input = UnityEngine.Input;

namespace Sources.ECS.BaseInteractions {
    public class InputSystem : IEcsRunSystem {
        /// <summary>
        /// This is system for updating global runtime data's input
        /// </summary>
        private RuntimeData runtimeData;
        private EcsWorld world;

        public void Run() {
            runtimeData.Input.Position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            runtimeData.Input.Primary = Input.GetMouseButton(0);
            runtimeData.Input.Secondary = Input.GetMouseButton(1);
        }
    }
}
