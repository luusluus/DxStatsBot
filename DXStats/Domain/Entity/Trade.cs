namespace DXStats.Domain.Entity
{
    public class Trade
    {
        public long Id { get; set; }
        public string TradeId { get; set; }
        public long Timestamp { get; set; }
        public string FeeTxId { get; set; }
        public string NodePubKey { get; set; }
        public Coin Taker { get; set; }
        public string TakerId { get; set; }
        public decimal TakerSize { get; set; }
        public Coin Maker { get; set; }
        public string MakerId { get; set; }
        public decimal MakerSize { get; set; }
        public decimal PriceUSD { get; set; }
        public decimal PriceBTC { get; set; }
        public decimal PriceBLOCK { get; set; }
    }
}
