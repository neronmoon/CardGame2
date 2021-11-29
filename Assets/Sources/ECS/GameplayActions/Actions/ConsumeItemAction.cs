using System.Collections.Generic;
using Leopotam.Ecs;
using Sources.Database.DataObject;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;
using Sources.ECS.GameplayActions.Components;

namespace Sources.ECS.GameplayActions.Actions {
    public class ConsumeItemAction :IGameplayMoveAction {
        public bool ShouldAct(EcsEntity entity, EcsEntity target) => target.Has<ConsumableItem>() && target.Has<CardEffects>();

        public object[] Act(EcsEntity entity, EcsEntity target) {
            List<ItemEffect> effects = target.Get<CardEffects>().Effects;
            List<object> components = new(effects.Count);
            foreach (ItemEffect effect in effects) {
                switch (effect.Name) {
                    case "Heal":
                        components.Add(new Heal { Amount = (int)effect.Value });
                        break;
                }
            }

            return components.ToArray();
        }
    }
}
