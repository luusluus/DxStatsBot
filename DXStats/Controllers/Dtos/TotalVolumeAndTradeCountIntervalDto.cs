using System;
using System.Collections.Generic;

namespace DXStats.Controllers.Dtos
{
    public class TotalVolumeAndTradeCountIntervalDto
    {
        public DateTime Timestamp { get; set; }
        public Dictionary<string, decimal> Volumes { get; set; }
        public int TradeCount { get; set; }
    }
}
