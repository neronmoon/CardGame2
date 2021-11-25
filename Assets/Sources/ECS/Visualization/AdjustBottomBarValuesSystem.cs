using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components.Gameplay;
using Sources.Unity;
using UnityEngine;

namespace Sources.ECS.Visualization {
    public class AdjustBottomBarValuesSystem : IEcsRunSystem {
        /// <summary>
        /// Sets bottom bar system values
        /// </summary>
        private SceneData sceneData;

        private EcsFilter<Player, Inventory, Health, MaxHealth> playerFilter;

        public void Run() {
            BottomBarView view = sceneData.BottomBarView;
            foreach (int idx in playerFilter) {
                EcsEntity player = playerFilter.GetEntity(idx);

                view.SetHealthCounterValues(playerFilter.Get3(idx).Value, playerFilter.Get4(idx).Value);
            }
        }
    }
}
