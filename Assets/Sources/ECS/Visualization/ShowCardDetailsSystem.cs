using Leopotam.Ecs;
using Sources.Data;
using Sources.ECS.Components;
using Sources.ECS.Components.Events;
using Sources.Unity;

namespace Sources.ECS.Visualization {
    public class ShowCardDetailsSystem : IEcsRunSystem {
        /// <summary>
        /// This system shows card details screen if card is double clicked
        /// </summary>
        private EcsFilter<PlayableCard, DoubleClickedEvent> cards;

        private SceneData sceneData;

        public void Run() {
            CardDetailsView view = sceneData.CardDetailsView;
            foreach (int idx in cards) {
                view.gameObject.SetActive(true);
                view.SetCard(cards.GetEntity(idx));
                view.Show();
            }
        }
    }
}
