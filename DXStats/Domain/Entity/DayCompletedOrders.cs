namespace DXStats.Domain.Entity
{
    public class DayCompletedOrders
    {
        public int Id { get; set; }
        public Coin Coin { get; set; }

        public Snapshot Snapshot { get; set; }
        public int Count { get; set; }

    }
}
