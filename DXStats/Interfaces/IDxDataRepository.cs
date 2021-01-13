using DXStats.Domain.Dto;
using DXStats.Domain.Entity;
using DXStats.Enums;
using System.Collections.Generic;

namespace DXStats.Interfaces
{
    public interface IDxDataRepository
    {
        void AddDailySnapshot(List<CompletedOrderCount> completedOrders, List<CoinTradeStatistics> coinTradeStatistics);

        DayVolume GetTotalVolumeAndTradesByElapsedTime(ElapsedTime elapsedTime);
        Dictionary<string, DayVolume> GetTotalVolumeAndTradesByCoinAndElapsedTime(ElapsedTime elapsedTime);
        Dictionary<string, int> GetTotalCompletedOrdersByElapsedTime(ElapsedTime elapsedTime);

        List<TotalVolumeAndTradeCountInterval> GetVolumeAndTradeCountByElapsedTime(ElapsedTime elapsedTime);

        List<TotalVolumeAndTradeCountInterval> GetVolumeAndTradeCountByElapsedTimeAndCoin(ElapsedTime elapsedTime, string coin);

        void AddCoin(Coin coin);
        void RemoveCoin(string coin);
        List<Coin> GetCoins();

    }
}
