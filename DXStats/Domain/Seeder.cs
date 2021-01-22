using Blocknet.Lib.Services.Coins.Blocknet.XBridge;
using DXStats.Domain.Dto;
using DXStats.Domain.Entity;
using DXStats.Interfaces;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.EntityFrameworkCore.Migrations;
using MoreLinq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading;

namespace DXStats.Domain
{
    public class Seeder : ISeeder
    {
        static readonly HashSet<string> cryptocompareUnsupportedCoins = new HashSet<string>()
        {
            "ABET", "AEX", "AGM", "APR", "AUS", "BAD", "BZX", "CDZC", "CHN", "CIV", "CNMC", "DVT", "FGC", "GEEK", "GMCN", "GXX", "HASH", "HATCH", "JIYOX", "KYDC", "LPC", "MERGE", "MLM", "MNP", "NORT", "NYX", "ODIN", "OHMC", "OPCX", "PHL", "QBIC", "SUB1X", "XN", "REEX"}; // Temporary row. Remove when cryptocompare api is updated

        static readonly HashSet<string> xbridgeNoLongerSupportedCoins = new HashSet<string>() { "OHM", "BWK", "XST", "CCBC", "MPWR", "KNG", "MRX", "TRB" };

        const string ENDPOINT_GECKO = "https://api.coingecko.com/api/v3/";

        private readonly List<CoinGeckoCoin> _coinGeckoCoins;
        private readonly HttpClient _httpClientCoinGecko;

        public Seeder()
        {
            _httpClientCoinGecko = new HttpClient();
            _httpClientCoinGecko.BaseAddress = new Uri(ENDPOINT_GECKO);
            _coinGeckoCoins = getCoinGeckoCoins();
            cryptocompareUnsupportedCoins.UnionWith(xbridgeNoLongerSupportedCoins);
        }

        private List<CoinGeckoCoin> getCoinGeckoCoins()
        {
            var coinListResponse = _httpClientCoinGecko.GetAsync("coins/list").Result;

            var coinListResponseContent = coinListResponse.Content;

            // by calling .Result you are synchronously reading the result
            string coinListResponseString = coinListResponseContent.ReadAsStringAsync().Result;

            return JsonConvert.DeserializeObject<List<CoinGeckoCoin>>(coinListResponseString);

        }

        private int getBlockCount()
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("https://service-explorer.core.cloudchainsinc.com/api/blocknet/GetBlockCount").Result;

                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;
                int blockcount = 0;

                int.TryParse(responseString, out blockcount);

                return blockcount;
            }
        }

        private List<GetTradingDataResponse> getLatestTradingData(int blocks)
        {
            using (var client = new HttpClient())
            {
                var response = client.GetAsync("https://service-explorer.core.cloudchainsinc.com/api/dx/GetTradingData?blocks=" + blocks).Result;

                var responseContent = response.Content;

                // by calling .Result you are synchronously reading the result
                string responseString = responseContent.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<List<GetTradingDataResponse>>(responseString);
            }
        }

        private List<TradingDataResponse> loadData()
        {
            var tradingDataResponse = new List<TradingDataResponse>();

            var currentWorkingDirectory = Environment.CurrentDirectory;

            var tradesZipFilePath = Path.Combine(currentWorkingDirectory, "trades_with_price.zip");

            ZipFile.ExtractToDirectory(tradesZipFilePath, currentWorkingDirectory, true);

            using (StreamReader r = new StreamReader(Path.Combine(currentWorkingDirectory, "trades_with_price.json")))
            {
                string json = r.ReadToEnd();
                tradingDataResponse = JsonConvert.DeserializeObject<List<TradingDataResponse>>(json);
            }

            return tradingDataResponse;
        }

        private object[,] MapListToRectangularArray(List<TradingDataResponse> trades, long IdStart)
        {
            const int NUM_COLUMNS = 12;
            var tradeData = new object[trades.Count, NUM_COLUMNS];

            for (int i = 0; i < trades.Count; i++)
            {
                var trade = trades[i];
                var tradeObject = new object[NUM_COLUMNS]
                    {
                        Convert.ToInt64(IdStart + i+1),
                        trade.Id,
                        trade.Timestamp,
                        trade.FeeTxId,
                        trade.NodePubKey,
                        trade.Taker,
                        trade.TakerSize,
                        trade.Maker,
                        trade.MakerSize,
                        trade.PriceUSD,
                        trade.PriceBTC,
                        trade.PriceBLOCK
                    };
                for (int j = 0; j < NUM_COLUMNS; j++)
                {
                    tradeData[i, j] = tradeObject[j];
                }
            }
            return tradeData;
        }

        private dynamic getCoinGeckoPriceHistory(string maker, string date)
        {
            int backoff = 1;
            while (true)
            {
                try
                {
                    var coin = _coinGeckoCoins.FirstOrDefault(c => c.Symbol.Equals(maker.ToLower()));

                    var historyResponse = _httpClientCoinGecko.GetAsync("coins/" + coin.Id + "/history?date=" + date).Result;

                    if (!historyResponse.IsSuccessStatusCode)
                    {
                        backoff *= 2;
                        Console.WriteLine("Rate Limit: Sleeping " + backoff + " sec");
                        Thread.Sleep(backoff * 1000);
                    }
                    else
                    {
                        var historyResponseContent = historyResponse.Content;

                        // by calling .Result you are synchronously reading the result
                        string historyResponseString = historyResponseContent.ReadAsStringAsync().Result;

                        return JsonConvert.DeserializeObject<dynamic>(historyResponseString);
                    }
                }
                catch (ArgumentNullException)
                {
                    throw;
                }
            }
        }
        private CoinGeckoPrice parseCoinGeckoPrices(dynamic history)
        {
            try
            {
                return new CoinGeckoPrice
                {
                    BTC = (decimal)history["market_data"]["current_price"]["btc"],
                    USD = (decimal)history["market_data"]["current_price"]["usd"]
                };
            }
            catch (RuntimeBinderException)
            {

                return new CoinGeckoPrice
                {
                    BTC = 0m,
                    USD = 0m
                };
            }
        }

        private TradingDataResponse getTradingDataWithPrice(GetTradingDataResponse tradingData)
        {
            var maker = tradingData.Maker;
            var date = UnixTimeStampToDateTime(tradingData.Timestamp).ToString("dd-MM-yyyy");

            int sleepTime;

            var trade = new TradingDataResponse
            {
                Maker = tradingData.Maker,
                MakerSize = tradingData.MakerSize,
                Taker = tradingData.Taker,
                TakerSize = tradingData.TakerSize,
                FeeTxId = tradingData.FeeTxId,
                Id = tradingData.Id,
                NodePubKey = tradingData.NodePubKey,
                Timestamp = tradingData.Timestamp
            };

            try
            {
                if (maker.Equals("BLOCK"))
                {
                    var historyBLOCK = getCoinGeckoPriceHistory(maker, date);

                    CoinGeckoPrice coinGeckoPrice = parseCoinGeckoPrices(historyBLOCK);

                    trade.PriceBLOCK = 1;
                    trade.PriceBTC = coinGeckoPrice.BTC;
                    trade.PriceUSD = coinGeckoPrice.USD;

                    sleepTime = 600;
                }
                else if (cryptocompareUnsupportedCoins.Contains(maker))
                {
                    Console.WriteLine(maker + " not xbridge supported");
                    trade.PriceBLOCK = 0m;
                    trade.PriceBTC = 0m;
                    trade.PriceUSD = 0m;
                    sleepTime = 0;
                }
                else // BTC and others
                {
                    var historyCoin = getCoinGeckoPriceHistory(maker, date);

                    CoinGeckoPrice coinGeckoPrice = parseCoinGeckoPrices(historyCoin);

                    trade.PriceBTC = coinGeckoPrice.BTC;
                    trade.PriceUSD = coinGeckoPrice.USD;

                    var historyBLOCK = getCoinGeckoPriceHistory(maker, date);

                    CoinGeckoPrice coinGeckoPriceBLOCK = parseCoinGeckoPrices(historyBLOCK);

                    trade.PriceBLOCK = coinGeckoPrice.BTC * (1 / coinGeckoPriceBLOCK.USD);

                    sleepTime = 1200;
                }
            }
            catch (ArgumentNullException)
            {
                Console.WriteLine(maker + " not in coingecko");
                trade.PriceBLOCK = 0m;
                trade.PriceBTC = 0m;
                trade.PriceUSD = 0m;
                sleepTime = 600;
            }

            Thread.Sleep(sleepTime);
            return trade;
        }

        public void Seed(MigrationBuilder migrationBuilder)
        {
            var tradingDataResponse = loadData();

            var blockcount = getBlockCount();

            // 1802665 block after snapshot
            var latestTradingData = getLatestTradingData(blockcount - 1802665);

            var latestTradingDataWithPrice = latestTradingData.Select(getTradingDataWithPrice).ToList();

            tradingDataResponse
                .AddRange(latestTradingDataWithPrice);

            var coinData = tradingDataResponse
                .GroupBy(t => t.Maker)
                .Select(g => new Coin
                {
                    Id = g.Key,
                    NumberOfTrades = g.Count(t => t.Maker.Equals(g.Key)),
                    Volume = g.Sum(t => t.MakerSize),
                    VolumeBLOCK = g.Sum(t => t.MakerSize * t.PriceBLOCK),
                    VolumeUSD = g.Sum(t => t.MakerSize * t.PriceUSD),
                    VolumeBTC = g.Sum(t => t.MakerSize * t.PriceBTC),
                })
                .ToList();

            var setPrecision = new NumberFormatInfo { NumberDecimalDigits = 10 };
            // insert all coins with volume and numberoftrades
            foreach (var coin in coinData)
            {
                migrationBuilder.InsertData(
                    table: "Coins",
                    columns: new[] { "Id", "NumberOfTrades", "SupportedSince", "Volume", "VolumeUSD", "VolumeBTC", "VolumeBLOCK" },
                    values: new object[] {
                        coin.Id,
                        coin.NumberOfTrades,
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        coin.Volume.ToString("N", setPrecision),
                        coin.VolumeUSD.ToString("N", setPrecision),
                        coin.VolumeBTC.ToString("N", setPrecision),
                        coin.VolumeBLOCK.ToString("N", setPrecision)
                    });
            }

            var coinsList = tradingDataResponse
                .SelectMany(t => new string[] { t.Maker, t.Taker })
                .GroupBy(g => g)
                .Select(g => new string(g.Key))
                .ToList();

            var coinsWithNoTrades = coinsList.Except(coinData.Select(c => c.Id)).ToList();

            // insert all coins (that were on taker side) with no volume and no trades 
            foreach (var coin in coinsWithNoTrades)
            {
                migrationBuilder.InsertData(
                    table: "Coins",
                    columns: new[] { "Id", "NumberOfTrades", "SupportedSince", "Volume", "VolumeUSD", "VolumeBTC", "VolumeBLOCK" },
                    values: new object[] {
                        coin,
                        0,
                        new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                        0.0m.ToString("N", setPrecision),
                        0.0m.ToString("N", setPrecision),
                        0.0m.ToString("N", setPrecision),
                        0.0m.ToString("N", setPrecision)
                    });
            }

            tradingDataResponse = tradingDataResponse
                .OrderBy(t => t.Timestamp)
                .GroupBy(tr => tr.Id)
                .Select(g => g.Last())
                .ToList();

            int batchSize = 1000;
            long idStart = 0;

            // insert all trades
            foreach (IEnumerable<TradingDataResponse> batchTrades in tradingDataResponse.Batch(batchSize))
            {
                var tradeData = MapListToRectangularArray(batchTrades.ToList(), idStart);
                idStart += batchSize;

                migrationBuilder.InsertData(
                    table: "Trades",
                    columns: new[] { "Id", "TradeId", "Timestamp", "FeeTxId", "NodePubKey", "TakerId", "TakerSize", "MakerId", "MakerSize", "PriceUSD", "PriceBTC", "PriceBLOCK" },
                    values: tradeData
                );
            }
        }

        public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public void Dispose()
        {
            _httpClientCoinGecko.Dispose();
        }
    }

    class CoinGeckoPrice
    {
        public decimal USD { get; set; }
        public decimal BTC { get; set; }
    }

    class CoinGeckoCoin
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
    }
}
