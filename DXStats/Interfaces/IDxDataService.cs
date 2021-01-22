using Blocknet.Lib.Services.Coins.Blocknet.XBridge;
using DXStats.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DXStats.Interfaces
{
    public interface IDxDataService
    {
        Task<List<GetTradingDataResponse>> GetTradingData(int blocks);

        Task<List<OpenOrdersPerMarket>> GetOpenOrdersPerMarket();
    }
}
