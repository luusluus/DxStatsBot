using Blocknet.Lib.Services.Coins.Blocknet.XBridge;
using DXStats.Domain.Dto;
using DXStats.Domain.Entity;
using DXStats.Enums;
using System.Collections.Generic;

namespace DXStats.Interfaces
{
    public interface IDxDataRepository
    {
        void AddTradingData(List<GetTradingDataResponse> tradingData);

        List<Trade> GetTradingData(ElapsedTime elapsedTime);

        void AddCoinStatistics(List<GetTradingDataResponse> tradingData);

        CoinTradeStatistics GetTotalVolumeAndTradesByElapsedTime(ElapsedTime elapsedTime);
        Dictionary<string, CoinTradeStatistics> GetTotalVolumeAndTradesByCoinAndElapsedTime(ElapsedTime elapsedTime);
        Dictionary<string, int> GetTotalCompletedOrdersByElapsedTime(ElapsedTime elapsedTime);

        List<CoinTradeStatisticsInterval> GetVolumeAndTradeCountByElapsedTime(ElapsedTime elapsedTime);

        List<CoinTradeStatisticsInterval> GetVolumeAndTradeCountByElapsedTimeAndCoin(ElapsedTime elapsedTime, string coin);


    }
}
