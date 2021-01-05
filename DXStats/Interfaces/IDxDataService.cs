using DXStats.Domain.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DXStats.Interfaces
{
    public interface IDxDataService
    {
        Task<List<CompletedOrderCount>> GetOneDayCompletedOrders();

        Task<List<CoinTradeStatistics>> GetOneDayTotalVolumePerCoin(string units);

        Task<List<CoinVolume>> GetOneDayTotalVolume(string coin, string units);

        Task<int> GetOneDayTotalTradesCount();

        Task<List<OpenOrdersPerMarket>> GetOpenOrdersPerMarket();
    }
}
