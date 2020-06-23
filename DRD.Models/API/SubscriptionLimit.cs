namespace DRD.Models.API
{
    public class SubscriptionLimit
    {
        public LimitItem Old { get; set; }
        public LimitItem New { get; set; }

        public SubscriptionLimit()
        {
            Old = new LimitItem();
            New = new LimitItem();
        }

        public class LimitItem
        {
            // feature not used yet
            public long StorageLimit { get; set; }
            public long TotalStorage { get; set; }
            public int AdminLimit { get; set; }
            public int TotalAdmin { get; set; }
        }
    }
}


