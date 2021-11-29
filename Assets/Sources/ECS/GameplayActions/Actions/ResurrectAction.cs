using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Database.DataObject;
using Sources.ECS.Components.Gameplay;

namespace Sources.ECS.GameplayActions.Actions {
    public class ResurrectAction : IGameplayAction {
        private ItemEffectsProcessor processor;

        public ResurrectAction() {
            processor = new ItemEffectsProcessor();
        }

        public bool ShouldAct(EcsEntity entity) => entity.Has<Health>() &&
                                                   entity.Get<Health>().Value <= 0 &&
                                                   entity.Get<Inventory>().HasItemWithEffect(ItemEffectType.Resurrection);

        public object[] Act(EcsEntity entity) {
            Inventory inventory = entity.Get<Inventory>();
            Item item = inventory.TakeFirstItemWithEffect(ItemEffectType.Resurrection);

            List<object> components = new() { inventory };
            components.AddRange(processor.ProcessItem(item, entity));
            return components.ToArray();
        }
    }
}
