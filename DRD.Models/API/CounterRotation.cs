namespace DRD.Models.API
{
    public class CounterRotation
    {
        public Item Old { get; set; }
        public Item New { get; set; }

        public CounterRotation()
        {
            Old = new Item();
            New = new Item();
        }

        public class Item
        {
            public int InProgress { get; set; }
            public int Completed { get; set; }
            public int Rejected { get; set; }
            // feature not available yet
            public int NotStarted { get; set; }
        }
    }
}


