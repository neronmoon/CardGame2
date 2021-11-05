using UnityEngine;

namespace Sources.Data.Gameplay.Items {
    public interface IConsumableItemType { };
    public interface IEquippableItemType { };

    public abstract class Item : ScriptableObject {
        public string Name;
        public string Description;
        public Sprite Sprite;
    }
}
