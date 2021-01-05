﻿using DXStats.Domain.Dto;
using DXStats.Domain.Entity;
using System.Collections.Generic;

namespace DXStats.Interfaces
{
    public interface IDxDataRepository
    {
        void AddDailySnapshot(List<CompletedOrderCount> completedOrders, List<CoinTradeStatistics> coinTradeStatistics);

        DayVolume GetTotalVolumeAndTradesByWeek();
        Dictionary<string, DayVolume> GetTotalVolumeAndTradesByWeekAndCoin();
        Dictionary<string, int> GetCompletedOrdersByWeek();
        void AddCoin(Coin coin);
        void RemoveCoin(string coin);

        List<Coin> GetCoins();


        //Task<List<OpenOrdersPerMarket>> GetOpenOrdersPerMarket();

        //Task AddOpenOrdersPerMarket(List<OpenOrdersPerMarket> openOrdersPerMarkets);

        //Task<List<CompletedOrderCount>> GetCompletedOrders();

        //Task AddCompletedOrders(List<CompletedOrderCount> completedOrderCounts);

        //Task<int> GetTotalTradesCount();

        //Task AddTotalTradesCount(int totalTradesCount);

        //Task<List<CoinVolume>> GetTotalVolume(string coin, string units);

        //Task AddTotalVolume(List<CoinVolume> coinVolumes);

        //Task<List<CoinTradeStatistics>> GetTotalVolumePerCoin(string units);
        //Task AddTotalVolumePerCoin(List<CoinTradeStatistics> coinTradeStatistics);

    }
}
