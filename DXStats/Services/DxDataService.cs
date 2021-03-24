using Blocknet.Lib.Services.Coins.Blocknet.XBridge;
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

        public async Task<List<GetTradingDataResponse>> GetTradingData(int blocks)
        {
            string queryString = "GetTradingData?blocks=" + blocks;

            var tradingDataResponse = await _client.GetAsync(queryString);

            if (!tradingDataResponse.IsSuccessStatusCode)
                throw new ApplicationException(tradingDataResponse.ReasonPhrase);

            string tradingDataResult = await tradingDataResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<GetTradingDataResponse>>(tradingDataResult);
        }
    }
}
