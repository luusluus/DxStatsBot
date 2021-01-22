using System;

namespace DXStats.Domain.Entity
{
    public class Coin
    {
        public string Id { get; set; }
        public DateTime SupportedSince { get; set; }
        public int NumberOfTrades { get; set; }
        public decimal Volume { get; set; }
        public decimal VolumeUSD { get; set; }
        public decimal VolumeBTC { get; set; }
        public decimal VolumeBLOCK { get; set; }
    }
}
