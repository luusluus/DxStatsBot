using System.Collections.Generic;

namespace DXStats.Domain.Dto
{
    public class CoinTradeStatistics
    {
        public int NumberOfTrades { get; set; }
        public Dictionary<string, decimal> Volumes { get; set; }

    }
}
