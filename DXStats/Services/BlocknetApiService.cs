using DXStats.Domain.Dto;
using DXStats.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace DXStats.Services
{
    public class BlocknetApiService : IBlocknetApiService
    {
        private readonly HttpClient _client;

        public BlocknetApiService(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<OpenOrder>> DxGetOrders()
        {
            var openOrdersResponse = await _client.GetAsync("dxgetorders");

            if (!openOrdersResponse.IsSuccessStatusCode)
                throw new ApplicationException();

            string openOrdersResult = await openOrdersResponse.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<OpenOrder>>(openOrdersResult);
        }

        public async Task<List<string>> DxGetTokens()
        {
            var networkTokens = await _client.GetAsync("dxgetnetworktokens");

            if (!networkTokens.IsSuccessStatusCode)
                throw new ApplicationException();

            string networkTokensResult = await networkTokens.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<List<string>>(networkTokensResult);
        }
    }
}
