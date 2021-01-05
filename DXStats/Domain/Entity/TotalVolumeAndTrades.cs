using System.Collections.Generic;

namespace DXStats.Domain.Entity
{
    public class TotalVolumeAndTrades
    {
        public int TotalNumberOfTrades { get; set; }
        public Dictionary<string, decimal> TotalVolumes { get; set; }
    }
}
