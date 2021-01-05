namespace DXStats.Controllers.Dtos
{
    public class TotalVolumePerCoinDto
    {
        public int NumberOfTrades { get; set; }
        public decimal USD { get; set; }
        public decimal BTC { get; set; }
        public decimal BLOCK { get; set; }
        public decimal CustomCoin { get; set; }
    }
}
