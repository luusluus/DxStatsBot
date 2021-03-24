
using Blocknet.Lib.Services.Coins.Blocknet.XBridge;
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
        static readonly HashSet<string> cryptocompareUnsupportedCoins = new HashSet<string>()
        {
            "ABET", "AEX", "AGM", "APR", "AUS", "BAD", "BZX", "CDZC", "CHN", "CIV", "CNMC", "DVT", "FGC", "GEEK", "GMCN", "GXX", "HASH", "HATCH", "JIYOX", "KYDC", "LPC", "MERGE", "MLM", "MNP", "NORT", "NYX", "ODIN", "OHMC", "OPCX", "PHL", "QBIC", "SUB1X", "XN", "REEX"}; // Temporary row. Remove when cryptocompare api is updated

        static readonly HashSet<string> xbridgeNoLongerSupportedCoins = new HashSet<string>() { "OHM", "BWK", "XST", "CCBC", "MPWR", "KNG", "MRX", "TRB" };

        private readonly DxStatsDbContext _context;
        private readonly ICoinPriceService _coinPriceService;
        public DxDataRepository(DxStatsDbContext context, ICoinPriceService coinPriceService)
        {
            _context = context;
            _coinPriceService = coinPriceService;

            cryptocompareUnsupportedCoins.UnionWith(xbridgeNoLongerSupportedCoins);
        }

        public List<Trade> GetTradingData(ElapsedTime elapsedTime)
        {
            var past = getUnixTimestampFromElapsedTime(elapsedTime);

            return _context.Trades
                .AsQueryable()
                .Where(t => t.Timestamp >= past)
                .ToList();
        }

        public void AddTradingData(List<GetTradingDataResponse> tradingData)
        {

            var trades = tradingData
                .Where(td => !cryptocompareUnsupportedCoins.Contains(td.Maker) && !cryptocompareUnsupportedCoins.Contains(td.Taker))
                .Select(td =>
            {
                var coinPrices = _coinPriceService.GetCoinPrice(new List<string> { td.Maker.ToUpper() }, new List<string> { "BLOCK", "USD", "BTC" });
                var priceBLOCK = coinPrices[td.Maker]["BLOCK"];
                var priceBTC = coinPrices[td.Maker]["BTC"];
                var priceUSD = coinPrices[td.Maker]["USD"];

                return new Trade
                {
                    TradeId = td.Id,
                    Timestamp = td.Timestamp,
                    FeeTxId = td.FeeTxId,
                    MakerId = td.Maker,
                    MakerSize = td.MakerSize,
                    NodePubKey = td.NodePubKey,
                    TakerId = td.Taker,
                    TakerSize = td.TakerSize,
                    PriceBLOCK = priceBLOCK,
                    PriceBTC = priceBTC,
                    PriceUSD = priceUSD
                };
            });


            _context.Trades.AddRange(trades);
        }

        public void AddCoinStatistics(List<GetTradingDataResponse> tradingData)
        {
            var coinStats = tradingData
                .GroupBy(t => t.Maker)
                .Select(g =>
                {
                    decimal priceBLOCK = 0;
                    decimal priceBTC = 0;
                    decimal priceUSD = 0;
                    if (!cryptocompareUnsupportedCoins.Contains(g.Key))
                    {
                        var coinPrices = _coinPriceService.GetCoinPrice(new List<string> { g.Key.ToUpper() }, new List<string> { "BLOCK", "USD", "BTC" });
                        priceBLOCK = coinPrices[g.Key]["BLOCK"];
                        priceBTC = coinPrices[g.Key]["BTC"];
                        priceUSD = coinPrices[g.Key]["USD"];
                    }
                    return new Coin
                    {
                        Id = g.Key,
                        NumberOfTrades = g.Count(t => t.Maker.Equals(g.Key)),
                        Volume = g.Sum(t => t.MakerSize),
                        VolumeBLOCK = g.Sum(t => t.MakerSize * priceBLOCK),
                        VolumeUSD = g.Sum(t => t.MakerSize * priceUSD),
                        VolumeBTC = g.Sum(t => t.MakerSize * priceBTC)
                    };
                })
                .ToList();

            Coin coin;
            coinStats.ForEach(c =>
            {
                coin = _context.Coins.FirstOrDefault(db => db.Id.Equals(c.Id));

                if (coin != null)
                {
                    coin.NumberOfTrades += c.NumberOfTrades;
                    coin.Volume += c.Volume;
                    coin.VolumeUSD += c.VolumeUSD;
                    coin.VolumeBTC += c.VolumeBTC;
                    coin.VolumeBLOCK += c.VolumeBLOCK;
                    _context.Entry(coin).State = EntityState.Modified;

                }
                else
                {
                    coin = new Coin
                    {
                        NumberOfTrades = c.NumberOfTrades,
                        Id = c.Id,
                        SupportedSince = DateTime.Now,
                        Volume = c.Volume,
                        VolumeBLOCK = c.VolumeBLOCK,
                        VolumeBTC = c.VolumeBTC,
                        VolumeUSD = c.VolumeUSD
                    };
                    _context.Entry(coin).State = EntityState.Added;
                }
            });
        }

        public CoinTradeStatistics GetTotalVolumeAndTradesByElapsedTime(ElapsedTime elapsedTime)
        {
            if (elapsedTime.Equals(ElapsedTime.All))
            {
                var coins = _context.Coins
                    .AsEnumerable()
                    .ToList();

                return new CoinTradeStatistics
                {
                    NumberOfTrades = coins.Sum(c => c.NumberOfTrades),
                    Volumes = new Dictionary<string, decimal>
                    {
                        { "USD",  coins.Sum(c => c.VolumeUSD) },
                        { "BTC",  coins.Sum(c => c.VolumeBTC) },
                        { "BLOCK",  coins.Sum(c => c.VolumeBLOCK) }
                    }
                };
            }

            var past = getUnixTimestampFromElapsedTime(elapsedTime);

            var trades = _context.Trades
                .AsEnumerable()
                .Where(t => t.Timestamp >= past)
                .ToList();

            return new CoinTradeStatistics
            {
                NumberOfTrades = trades.Count(),
                Volumes = new Dictionary<string, decimal>
                {
                    { "USD",  trades.Sum(t => t.MakerSize * t.PriceUSD) },
                    { "BTC",  trades.Sum(t => t.MakerSize * t.PriceBTC) },
                    { "BLOCK",  trades.Sum(t => t.MakerSize * t.PriceBLOCK) }
                }
            };
        }

        public Dictionary<string, int> GetTotalCompletedOrdersByElapsedTime(ElapsedTime elapsedTime)
        {
            var past = getUnixTimestampFromElapsedTime(elapsedTime);

            return _context.Trades
                .AsEnumerable()
                .Where(t => t.Timestamp >= past)
                .SelectMany(t => new string[] { t.TakerId, t.MakerId })
                .GroupBy(g => g)
                .Select(g => new
                {
                    g.Key,
                    TotalCompletedOrders = g.Count()
                })
                .ToDictionary(g => g.Key, g => g.TotalCompletedOrders);
        }

        public Dictionary<string, CoinTradeStatistics> GetTotalVolumeAndTradesByCoinAndElapsedTime(ElapsedTime elapsedTime)
        {
            var past = getUnixTimestampFromElapsedTime(elapsedTime);

            return _context.Trades
                .AsEnumerable()
                .Where(t => t.Timestamp >= past)
                .GroupBy(t => t.MakerId)
                .Select(g => new
                {
                    g.Key,
                    VolumeSum = g.Sum(t => t.MakerSize),
                    VolumeSumUSD = g.Sum(t => t.MakerSize * t.PriceUSD),
                    VolumeSumBLOCK = g.Sum(t => t.MakerSize * t.PriceBLOCK),
                    VolumeSumBTC = g.Sum(t => t.MakerSize * t.PriceBTC),
                    NumberOfTrades = g.Count(t => t.MakerId.Equals(g.Key))
                })
                .ToDictionary(g => g.Key, g =>
                {
                    var volumes = new Dictionary<string, decimal>
                    {
                        { "USD",  g.VolumeSumUSD },
                        { "BTC",  g.VolumeSumBTC },
                        { "BLOCK",  g.VolumeSumBLOCK }
                    };

                    if (!g.Key.Equals("BLOCK") && !g.Key.Equals("USD") && !g.Key.Equals("BTC"))
                        volumes.Add(g.Key, g.VolumeSum);

                    return new CoinTradeStatistics
                    {
                        NumberOfTrades = g.NumberOfTrades,
                        Volumes = volumes
                    };
                });
        }

        public List<CoinTradeStatisticsInterval> GetVolumeAndTradeCountByElapsedTime(ElapsedTime elapsedTime)
        {
            throw new NotImplementedException();
            //    var timespan = getTimeSpanFromElapsedTime(elapsedTime);

            //    var past = getDateTimeFromElapsedTime(elapsedTime);

            //    return
            //        _context.DayVolumes
            //        .Include(dv => dv.Snapshot)
            //        .Include(dv => dv.Coin)
            //        .Where(dv => dv.Snapshot.DateCreated >= past)
            //        .ToList()
            //        .GroupBy(dv => dv.Snapshot.DateCreated.Ticks / timespan.Ticks)
            //        .Select(g => new TotalVolumeAndTradeCountInterval
            //        {
            //            Timestamp = g.Last().Snapshot.DateCreated,
            //            USD = g.Sum(dv => dv.USD),
            //            BLOCK = g.Sum(dv => dv.BLOCK),
            //            BTC = g.Sum(dv => dv.BTC),
            //            CustomCoin = g.Sum(dv => dv.CustomCoin),
            //            TradeCount = g.Sum(dv => dv.NumberOfTrades)
            //        })
            //        .ToList();
        }

        public List<CoinTradeStatisticsInterval> GetVolumeAndTradeCountByElapsedTimeAndCoin(ElapsedTime elapsedTime, string coin)
        {
            throw new NotImplementedException();
            //    var timespan = getTimeSpanFromElapsedTime(elapsedTime);

            //    var past = getDateTimeFromElapsedTime(elapsedTime);

            //    return
            //        _context.DayVolumes
            //        .Include(dv => dv.Snapshot)
            //        .Include(dv => dv.Coin)
            //        .Where(dv => dv.Coin.Id.Equals(coin.ToUpper()) && dv.Snapshot.DateCreated >= past)
            //        .ToList()
            //        .GroupBy(dv => dv.Snapshot.DateCreated.Ticks / timespan.Ticks)

            //        .Select(g => new TotalVolumeAndTradeCountInterval
            //        {
            //            Timestamp = g.Last().Snapshot.DateCreated,
            //            USD = g.Sum(dv => dv.USD),
            //            BLOCK = g.Sum(dv => dv.BLOCK),
            //            BTC = g.Sum(dv => dv.BTC),
            //            CustomCoin = g.Sum(dv => dv.CustomCoin),
            //            TradeCount = g.Sum(dv => dv.NumberOfTrades)
            //        })
            //        .ToList();
        }

        private long getUnixTimestampFromElapsedTime(ElapsedTime elapsedTime)
        {
            long timestamp = DateTimeOffset.Now.ToUnixTimeSeconds();

            switch (elapsedTime)
            {
                case ElapsedTime.FiveMinutes:
                    timestamp -= (5 * 60);
                    break;
                case ElapsedTime.FifteenMinutes:
                    timestamp -= (15 * 60);
                    break;
                case ElapsedTime.Hour:
                    timestamp -= (60 * 60);
                    break;
                case ElapsedTime.TwoHours:
                    timestamp -= (2 * 60 * 60);
                    break;
                case ElapsedTime.Day:
                    timestamp -= (24 * 60 * 60);
                    break;
                case ElapsedTime.Week:
                    timestamp -= (7 * 24 * 60 * 60);
                    break;
                case ElapsedTime.Month:
                    timestamp -= (31 * 24 * 60 * 60);
                    break;
                case ElapsedTime.ThreeMonths:
                    timestamp -= (3 * 31 * 24 * 60 * 60);
                    break;
                case ElapsedTime.Year:
                    timestamp -= (12 * 31 * 24 * 60 * 60);
                    break;
                case ElapsedTime.All:
                    timestamp = DateTimeOffset.MinValue.ToUnixTimeSeconds();
                    break;

                default:
                    timestamp = DateTimeOffset.MinValue.ToUnixTimeSeconds();
                    break;
            }
            return timestamp;
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
