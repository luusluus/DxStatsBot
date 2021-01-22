using DXStats.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DXStats.Services
{
    public class CoinPriceService : ICoinPriceService
    {
        private readonly HttpClient _client;
        public CoinPriceService(HttpClient client)
        {
            _client = client;
        }
        public Dictionary<string, Dictionary<string, decimal>> GetCoinPrice(List<string> fromCoins, List<string> toCoins)
        {
            var pricesFullTask = Task.Run(async () => await _client.GetAsync($"prices_full?from_currencies={string.Join(",", fromCoins)}&to_currencies={string.Join(",", toCoins)}"));
            var pricesFullStringTask = Task.Run(async () => await pricesFullTask.Result.Content.ReadAsStringAsync());

            var coinPricing = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, Dictionary<string, dynamic>>>>(pricesFullStringTask.Result);

            var raw = coinPricing["RAW"];

            return raw.ToDictionary(bc => bc.Key, bc => bc.Value.ToDictionary(qc => qc.Key, qc => (decimal)qc.Value["PRICE"]));
        }
    }
}
