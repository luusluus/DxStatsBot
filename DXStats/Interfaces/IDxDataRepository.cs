using DXStats.Domain.Dto;
using DXStats.Domain.Entity;
using DXStats.Enums;
using System.Collections.Generic;

namespace DXStats.Interfaces
{
    public interface IDxDataRepository
    {
        void AddDailySnapshot(List<CompletedOrderCount> completedOrders, List<CoinTradeStatistics> coinTradeStatistics);

        DayVolume GetTotalVolumeAndTrades(TimeInterval timeInterval);
        Dictionary<string, DayVolume> GetTotalVolumeAndTradesByCoin(TimeInterval timeInterval);
        Dictionary<string, int> GetTotalCompletedOrders(TimeInterval timeInterval);
        void AddCoin(Coin coin);
        void RemoveCoin(string coin);

        List<Coin> GetCoins();

    }
}
