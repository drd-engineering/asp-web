namespace DRD.Models.View
{
    public class CounterInboxData
    {
        public CountedItem Old { get; set; }
        public CountedItem New { get; set; }

        public CounterInboxData()
        {
            Old = new CountedItem();
            New = new CountedItem();
        }

        /// <summary>
        /// Counter inbox data. You can add more item that want to count in later here
        /// </summary>
        public class CountedItem
        {
            public int Read { get; set; }
            public int Unread { get; set; }
        }
    }
}


