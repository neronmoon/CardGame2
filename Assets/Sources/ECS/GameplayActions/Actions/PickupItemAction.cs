using Leopotam.Ecs;
using Sources.ECS.Components.Gameplay;
using Sources.ECS.Components.Gameplay.CardTypes;

namespace Sources.ECS.GameplayActions.Actions {
     public class PickupItemAction : IGameplayMoveAction {
         public bool ShouldAct(EcsEntity entity, EcsEntity target) => target.Has<EquippableItem>() && entity.Has<Inventory>();

         public object[] Act(EcsEntity entity, EcsEntity target) {
             return new object[] { entity.Get<Inventory>().Add(target.Get<EquippableItem>().Data) };
         }
     }
}
