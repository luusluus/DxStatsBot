using Newtonsoft.Json;

namespace DXStats.Domain.Dto
{
    public class TradingDataResponse
    {
        public long Timestamp { get; set; }
        [JsonProperty("fee_txid")]
        public string FeeTxId { get; set; }
        public string NodePubKey { get; set; }
        public string Id { get; set; }
        public string Taker { get; set; }
        [JsonProperty("taker_size")]
        public decimal TakerSize { get; set; }
        public string Maker { get; set; }
        [JsonProperty("maker_size")]
        public decimal MakerSize { get; set; }
        [JsonProperty("usd_price")]
        public decimal PriceUSD { get; set; }
        [JsonProperty("btc_price")]
        public decimal PriceBTC { get; set; }
        [JsonProperty("block_price")]
        public decimal PriceBLOCK { get; set; }
    }
}
