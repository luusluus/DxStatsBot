using DXStats.Domain.Dto;
using DXStats.Domain.Entity;
using DXStats.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DXStats.Persistence
{
    public class DxDataRepository : IDxDataRepository
    {
        private readonly DxStatsDbContext _context;
        public DxDataRepository(DxStatsDbContext context)
        {
            _context = context;
        }

        public void AddDailySnapshot(List<CompletedOrderCount> completedOrders, List<CoinTradeStatistics> coinTradeStatistics)
        {
            var snapshot = new Snapshot
            {
                DateCreated = DateTime.Now,

            };

            var coins = _context.Coins.ToList();


            var dayCompletedOrders = new List<DayCompletedOrders>();

            completedOrders.ForEach(co =>
            {
                var coin = coins.FirstOrDefault(c => c.Id.Equals(co.Coin));

                dayCompletedOrders.Add(new DayCompletedOrders
                {
                    Coin = coin,
                    Snapshot = snapshot,
                    Count = co.Count
                });
            });


            snapshot.DayCompletedOrders = dayCompletedOrders;

            var dayVolumes = new List<DayVolume>();

            coinTradeStatistics.ForEach(cs =>
            {
                if (cs.TradeCount > 0)
                {
                    var coin = coins.FirstOrDefault(c => c.Id.Equals(cs.Coin));

                    dayVolumes.Add(new DayVolume
                    {
                        Coin = coin,
                        Snapshot = snapshot,
                        NumberOfTrades = cs.TradeCount,
                        BLOCK = cs.Volumes.FirstOrDefault(v => v.Unit.Equals("BLOCK")).Volume,
                        BTC = cs.Volumes.FirstOrDefault(v => v.Unit.Equals("BTC")).Volume,
                        USD = cs.Volumes.FirstOrDefault(v => v.Unit.Equals("USD")).Volume,
                        CustomCoin = cs.Volumes.FirstOrDefault(v => v.Unit.Equals(coin.Id)).Volume
                    });
                }

            });

            snapshot.DayVolumes = dayVolumes;

            _context.Add(snapshot);
        }

        public void AddCoin(Coin coin)
        {
            _context.Coins.Add(coin);
        }

        public void RemoveCoin(string coin)
        {
            var existingCoin = _context.Coins.FirstOrDefault(c => c.Id.Equals(coin));

            if (existingCoin != null)
            {
                _context.Remove(existingCoin);
            }

        }

        public List<Coin> GetCoins()
        {
            return _context.Coins.ToList();
        }

        public DayVolume GetTotalVolumeAndTradesByWeek()
        {
            var lastweek = DateTime.Now.Date.AddDays(-7);
            var dayVolumes = _context.DayVolumes
                .Include(dv => dv.Snapshot)
                .Where(dv => dv.Snapshot.DateCreated >= lastweek)
                .ToList();

            return new DayVolume
            {
                USD = dayVolumes.Sum(dv => dv.USD),
                BTC = dayVolumes.Sum(dv => dv.BTC),
                BLOCK = dayVolumes.Sum(dv => dv.BLOCK),
                NumberOfTrades = dayVolumes.Sum(dv => dv.NumberOfTrades)
            };
        }

        public Dictionary<string, int> GetCompletedOrdersByWeek()
        {
            var lastweek = DateTime.Now.Date.AddDays(-7);

            return _context.DayCompletedOrders
                .Include(co => co.Snapshot)
                .Include(co => co.Coin)
                .Where(s => s.Snapshot.DateCreated >= lastweek)
                .ToList()
                .GroupBy(dv => dv.Coin.Id)
                .Select(g => new
                {
                    g.Key,
                    SumExecutedOrders = g.Sum(dv => dv.Count)
                })
                .ToDictionary(g => g.Key, g => g.SumExecutedOrders);
        }

        public Dictionary<string, DayVolume> GetTotalVolumeAndTradesByWeekAndCoin()
        {
            var lastweek = DateTime.Now.Date.AddDays(-7);

            return _context.DayVolumes
                 .Include(dv => dv.Snapshot)
                 .Include(dv => dv.Coin)
                 .Where(dv => dv.Snapshot.DateCreated >= lastweek)
                 .ToList()
                 .GroupBy(dv => dv.Coin.Id)
                 .Select(g => new
                 {
                     g.Key,
                     SumBLOCK = g.Sum(dv => dv.BLOCK),
                     SumBTC = g.Sum(dv => dv.BTC),
                     SumUSD = g.Sum(dv => dv.USD),
                     SumCustomCoin = g.Sum(dv => dv.CustomCoin),
                     SumNumberOfTrades = g.Sum(dv => dv.NumberOfTrades)
                 })
                 .ToDictionary(g => g.Key, g => new DayVolume
                 {
                     BLOCK = g.SumBLOCK,
                     BTC = g.SumBTC,
                     USD = g.SumUSD,
                     CustomCoin = g.SumCustomCoin,
                     NumberOfTrades = g.SumNumberOfTrades
                 });
        }
    }
}
