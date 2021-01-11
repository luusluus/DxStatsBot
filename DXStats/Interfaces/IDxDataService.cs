using DXStats.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DXStats.Interfaces
{
    public interface IDxDataService
    {
        Task<List<CompletedOrderCount>> GetTotalCompletedOrders();

        Task<List<CoinTradeStatistics>> GetTotalVolumePerCoin(string units);

        Task<List<CoinVolume>> GetTotalVolume(string coin, string units);

        Task<int> GetTotalTradesCount();

        Task<List<OpenOrdersPerMarket>> GetOpenOrdersPerMarket();
    }
}
