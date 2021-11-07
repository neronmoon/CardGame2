using NaughtyAttributes;
using UnityEngine;

namespace Sources.Data.Gameplay.Items {
    public interface IConsumableItemType { };
    public interface IEquippableItemType { };

    public abstract class ItemData : GameplayData {
        public string Name;
        
        [ResizableTextArea]
        public string Description;
        
        [ShowAssetPreview]
        public Sprite Sprite;
    }
}
