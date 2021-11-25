using SQLite;

namespace Sources.Database.DataObject {
    public class Level : CardsContainer<Level> {
        [NotNull]
        public float Difficulty { get; set; }
        
        public int SubLevelCount { get; set; }

        public string Sprite { get; set; }
        
        [Ignore]
        public Item RewardItem {
            get => RewardItemRaw != null ? Item.First(x => x.Id == RewardItemRaw) : null;
            set => RewardItemRaw = value.Id;
        }

        [Column("Reward")]
        public int RewardItemRaw { get; set; }
    }
}
