using DXStats.Domain.Dto;
using DXStats.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DXStats.Services
{
    public class DxDataService : IDxDataService
    {
        private readonly HttpClient _client;

        public DxDataService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<OpenOrdersPerMarket>> GetOpenOrdersPerMarket()
        {
            var openOrdersPerMarketResponse = await _client.GetAsync("GetOpenOrdersPerMarket");

            if (!openOrdersPerMarketResponse.IsSuccessStatusCode)
                throw new ApplicationException();

            string openOrdersPerMarketResult = await openOrdersPerMarketResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<OpenOrdersPerMarket>>(openOrdersPerMarketResult);
        }

        public async Task<List<CompletedOrderCount>> GetTotalCompletedOrders()
        {
            var completedOrdersResponse = await _client.GetAsync("GetTotalCompletedOrders&timeinterval=fifteenminutes");

            if (!completedOrdersResponse.IsSuccessStatusCode)
                throw new ApplicationException();

            string completedOrdersResponseResult = await completedOrdersResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CompletedOrderCount>>(completedOrdersResponseResult);
        }

        public async Task<int> GetTotalTradesCount()
        {
            var totalTradeCountResponse = await _client.GetAsync("GetTotalTradesCount&timeinterval=fifteenminutes");

            if (!totalTradeCountResponse.IsSuccessStatusCode)
                throw new ApplicationException();

            string totalTradeCountResult = await totalTradeCountResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<int>(totalTradeCountResult);
        }

        public async Task<List<CoinVolume>> GetTotalVolume(string coin, string units)
        {
            string queryString = "GetTotalVolume?coin=" + coin + "&units=" + units + "&timeinterval=fifteenminutes";

            var totalVolumeResponse = await _client.GetAsync(queryString);

            if (!totalVolumeResponse.IsSuccessStatusCode)
                throw new ApplicationException();

            string totalVolumeResult = await totalVolumeResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CoinVolume>>(totalVolumeResult);
        }

        public async Task<List<CoinTradeStatistics>> GetTotalVolumePerCoin(string units)
        {
            string queryString = "GetTotalVolumePerCoin?units=" + units + "&timeinterval=fifteenminutes";

            var totalVolumePerTradedCoinResponse = await _client.GetAsync(queryString);

            if (!totalVolumePerTradedCoinResponse.IsSuccessStatusCode)
                throw new ApplicationException();

            string totalVolumePerTradedCoinResult = await totalVolumePerTradedCoinResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<CoinTradeStatistics>>(totalVolumePerTradedCoinResult);
        }
    }
}
