using Leopotam.Ecs;
using Sources.ECS.Components.Gameplay.CardTypes;

namespace Sources.ECS.GameplayActions.Actions {
    public class ConsumeItemAction : IGameplayMoveAction {
        private readonly ItemEffectsProcessor processor;

        public ConsumeItemAction() {
            processor = new ItemEffectsProcessor();
        }

        public bool ShouldAct(EcsEntity entity, EcsEntity target) => target.Has<ConsumableItem>();

        public object[] Act(EcsEntity entity, EcsEntity target) {
            return processor.ProcessItem(target.Get<ConsumableItem>().Data, target);
        }
    }
}
