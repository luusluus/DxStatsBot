using DXStats.Domain.Dto;
using DXStats.Domain.Entity;
using DXStats.Enums;
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

        public DayVolume GetTotalVolumeAndTradesByElapsedTime(ElapsedTime elapsedTime)
        {
            var past = getDateTimeFromElapsedTime(elapsedTime);

            var dayVolumes = _context.DayVolumes
                .Include(dv => dv.Snapshot)
                .Where(dv => dv.Snapshot.DateCreated >= past)
                .ToList();

            return new DayVolume
            {
                USD = dayVolumes.Sum(dv => dv.USD),
                BTC = dayVolumes.Sum(dv => dv.BTC),
                BLOCK = dayVolumes.Sum(dv => dv.BLOCK),
                NumberOfTrades = dayVolumes.Sum(dv => dv.NumberOfTrades)
            };
        }

        public Dictionary<string, int> GetTotalCompletedOrdersByElapsedTime(ElapsedTime elapsedTime)
        {
            var past = getDateTimeFromElapsedTime(elapsedTime);

            var dco = _context.DayCompletedOrders
                .Include(co => co.Snapshot)
                .Include(co => co.Coin)
                .Where(s => s.Snapshot.DateCreated >= past)
                .ToList()
                .GroupBy(dv => dv.Coin.Id)
                .Select(g => new
                {
                    g.Key,
                    SumExecutedOrders = g.Sum(dv => dv.Count)
                })
                .ToDictionary(g => g.Key, g => g.SumExecutedOrders);

            var coins = GetCoins();

            var nonZeroVolumeCoins = new List<string>(dco.Keys);

            var zeroVolumeCoins = coins.Select(c => c.Id).Except(nonZeroVolumeCoins).ToList();

            zeroVolumeCoins.ForEach(zvc => dco.Add(zvc, 0));

            return dco;
        }

        public Dictionary<string, DayVolume> GetTotalVolumeAndTradesByCoinAndElapsedTime(ElapsedTime elapsedTime)
        {
            var past = getDateTimeFromElapsedTime(elapsedTime);

            var dv = _context.DayVolumes
                 .Include(dv => dv.Snapshot)
                 .Include(dv => dv.Coin)
                 .Where(dv => dv.Snapshot.DateCreated >= past)
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

            var coins = GetCoins();

            var nonZeroVolumeCoins = new List<string>(dv.Keys);

            var zeroVolumeCoins = coins.Select(c => c.Id).Except(nonZeroVolumeCoins).ToList();

            zeroVolumeCoins.ForEach(zvc => dv.Add(zvc, new DayVolume
            {
                BLOCK = 0,
                BTC = 0,
                USD = 0,
                NumberOfTrades = 0
            }));

            return dv;
        }

        public List<TotalVolumeAndTradeCountInterval> GetVolumeAndTradeCountByElapsedTime(ElapsedTime elapsedTime)
        {
            var timespan = getTimeSpanFromElapsedTime(elapsedTime);

            var past = getDateTimeFromElapsedTime(elapsedTime);

            return
                _context.DayVolumes
                .Include(dv => dv.Snapshot)
                .Include(dv => dv.Coin)
                .Where(dv => dv.Snapshot.DateCreated >= past)
                .ToList()
                .GroupBy(dv => dv.Snapshot.DateCreated.Ticks / timespan.Ticks)
                .Select(g => new TotalVolumeAndTradeCountInterval
                {
                    Timestamp = g.Last().Snapshot.DateCreated,
                    USD = g.Sum(dv => dv.USD),
                    BLOCK = g.Sum(dv => dv.BLOCK),
                    BTC = g.Sum(dv => dv.BTC),
                    CustomCoin = g.Sum(dv => dv.CustomCoin),
                    TradeCount = g.Sum(dv => dv.NumberOfTrades)
                })
                .ToList();
        }

        public List<TotalVolumeAndTradeCountInterval> GetVolumeAndTradeCountByElapsedTimeAndCoin(ElapsedTime elapsedTime, string coin)
        {
            var timespan = getTimeSpanFromElapsedTime(elapsedTime);

            var past = getDateTimeFromElapsedTime(elapsedTime);

            return
                _context.DayVolumes
                .Include(dv => dv.Snapshot)
                .Include(dv => dv.Coin)
                .Where(dv => dv.Coin.Id.Equals(coin) && dv.Snapshot.DateCreated >= past)
                .ToList()
                .GroupBy(dv => dv.Snapshot.DateCreated.Ticks / timespan.Ticks)

                .Select(g => new TotalVolumeAndTradeCountInterval
                {
                    Timestamp = g.Last().Snapshot.DateCreated,
                    USD = g.Sum(dv => dv.USD),
                    BLOCK = g.Sum(dv => dv.BLOCK),
                    BTC = g.Sum(dv => dv.BTC),
                    CustomCoin = g.Sum(dv => dv.CustomCoin),
                    TradeCount = g.Sum(dv => dv.NumberOfTrades)
                })
                .ToList();
        }

        private DateTime getDateTimeFromElapsedTime(ElapsedTime elapsedTime)
        {
            DateTime dateTime;

            switch (elapsedTime)
            {
                case ElapsedTime.FiveMinutes:
                    dateTime = DateTime.Now.AddMinutes(-5);
                    break;
                case ElapsedTime.FifteenMinutes:
                    dateTime = DateTime.Now.AddMinutes(-15);
                    break;
                case ElapsedTime.Hour:
                    dateTime = DateTime.Now.AddHours(-1);
                    break;
                case ElapsedTime.TwoHours:
                    dateTime = DateTime.Now.AddHours(-2);
                    break;
                case ElapsedTime.Day:
                    dateTime = DateTime.Now.AddDays(-1);
                    break;
                case ElapsedTime.Week:
                    dateTime = DateTime.Now.AddDays(-7);
                    break;
                case ElapsedTime.Month:
                    dateTime = DateTime.Now.AddDays(-31);
                    break;
                case ElapsedTime.ThreeMonths:
                    dateTime = DateTime.Now.AddDays(-93);
                    break;
                case ElapsedTime.Year:
                    dateTime = DateTime.Now.AddDays(-365);
                    break;
                case ElapsedTime.All:
                    dateTime = DateTime.MinValue;
                    break;

                default:
                    dateTime = DateTime.MinValue;
                    break;
            }
            return dateTime;
        }

        private TimeSpan getTimeSpanFromElapsedTime(ElapsedTime elapsedTime)
        {
            TimeSpan timespan;

            switch (elapsedTime)
            {
                case ElapsedTime.Day:
                    timespan = TimeSpan.FromMinutes(5);
                    break;
                case ElapsedTime.Week:
                    timespan = TimeSpan.FromMinutes(15);
                    break;
                case ElapsedTime.Month:
                    timespan = TimeSpan.FromHours(1);
                    break;
                case ElapsedTime.ThreeMonths:
                    timespan = TimeSpan.FromHours(2);
                    break;
                case ElapsedTime.Year:
                    timespan = TimeSpan.FromDays(1);
                    break;
                case ElapsedTime.All:
                    timespan = TimeSpan.FromDays(1);
                    break;
                default:
                    timespan = TimeSpan.Zero;
                    break;
            }
            return timespan;
        }
    }
}
