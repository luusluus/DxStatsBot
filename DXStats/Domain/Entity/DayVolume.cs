namespace DXStats.Domain.Entity
{
    public class DayVolume
    {
        public int Id { get; set; }
        public Coin Coin { get; set; }

        public Snapshot Snapshot { get; set; }

        public int NumberOfTrades { get; set; }
        public decimal USD { get; set; }
        public decimal BTC { get; set; }
        public decimal BLOCK { get; set; }
        public decimal CustomCoin { get; set; }
    }
}
