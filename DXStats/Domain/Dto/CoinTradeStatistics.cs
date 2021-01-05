using System.Collections.Generic;

namespace DXStats.Domain.Dto
{
    public class CoinTradeStatistics
    {
        public string Coin { get; set; }
        public List<CoinVolume> Volumes { get; set; }
        public int TradeCount { get; set; }
    }
}
