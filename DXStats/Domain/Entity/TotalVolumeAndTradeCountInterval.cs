using System;

namespace DXStats.Domain.Entity
{
    public class TotalVolumeAndTradeCountInterval
    {
        public DateTime Timestamp { get; set; }
        public decimal USD { get; set; }
        public decimal BTC { get; set; }
        public decimal BLOCK { get; set; }
        public decimal CustomCoin { get; set; }
        public int TradeCount { get; set; }
    }
}
