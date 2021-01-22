using System;
using System.Collections.Generic;

namespace DXStats.Domain.Dto
{
    public class CoinTradeStatisticsInterval
    {
        public DateTime Timestamp { get; set; }
        public Dictionary<string, decimal> Volumes { get; set; }
        public int TradeCount { get; set; }
    }
}
