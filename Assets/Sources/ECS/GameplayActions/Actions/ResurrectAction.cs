using System.Linq;
using Leopotam.Ecs;
using Sources.Database.DataObject;
using Sources.ECS.Components.Gameplay;

namespace Sources.ECS.GameplayActions.Actions {
    public class ResurrectAction : IGameplayAction {
        public bool ShouldAct(EcsEntity entity) => entity.Has<Health>() &&
                                                   entity.Get<Health>().Value <= 0 &&
                                                   entity.Get<Inventory>().HasWithEffect("Resurrection");

        public object[] Act(EcsEntity entity) {
            // Inventory inventory = entity.Get<Inventory>();
            // Item item = inventory.TakeOneWithEffect("Resurrection");

            // return new object[] {
            // inventory,
            // new Health { Value = (int)item.Effects.First(e => e.Name == "Resurrection").Value }
            // };
            return new object[] { };
        }
    }
}
