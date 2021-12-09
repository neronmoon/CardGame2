using Leopotam.Ecs;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using UnityEngine;

namespace Sources.ECS.Visualization {
    public class ShowCardDetailsSystem : IEcsRunSystem {
        /// <summary>
        /// This system shows card details screen if card is double clicked
        /// </summary>
        private EcsFilter<PlayableCard, DoubleClickedEvent> cards;

        public void Run() {
            foreach (int idx in cards) {
                 Debug.Log(cards.Get1(idx));
            }
        }
    }
}
