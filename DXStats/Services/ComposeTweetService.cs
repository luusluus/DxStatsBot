using DXStats.Enums;
using DXStats.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DXStats.Services
{
    public class ComposeTweetService : IComposeTweetService
    {
        private readonly IDxDataRepository _dxDataRepository;
        private readonly IBlocknetApiService _blocknetApiService;
        private readonly IDxDataService _dxDataService;

        static readonly List<string> units = new List<string>()
        {
            "BLOCK",
            "BTC",
            "USD"
        };

        public ComposeTweetService(IDxDataRepository dxDataRepository, IBlocknetApiService blocknetApiService, IDxDataService dxDataService)
        {
            _dxDataRepository = dxDataRepository;
            _blocknetApiService = blocknetApiService;
            _dxDataService = dxDataService;
        }


        public async Task<List<string>> ComposeOrdersAndActiveMarkets()
        {
            var TWEET_ORDERS_MAX = 12;

            var openOrders = await _blocknetApiService.DxGetOrders();

            var openOrdersPerMarket = await _dxDataService.GetOpenOrdersPerMarket();

            var tweets = new List<string>();

            var tweet = string.Empty;
            tweet += "Number of Open Orders: " + openOrders.Count() + "\n\n";

            // 12 lines of open order pairs max in each tweet

            int amountOfTweets = (int)Math.Ceiling((decimal)(openOrdersPerMarket.Count) / TWEET_ORDERS_MAX);

            var concatString = string.Empty;

            int idx = 1;
            for (int i = 0; i < openOrdersPerMarket.Count; i = i + TWEET_ORDERS_MAX)
            {
                var orders = openOrdersPerMarket.Skip(i).Take(TWEET_ORDERS_MAX).ToList();

                if (i > 0)
                    tweet = string.Empty;

                if (openOrdersPerMarket.Count > TWEET_ORDERS_MAX)
                    tweet += "Active Markets (" + idx++ + "/" + amountOfTweets + "):\n\n";

                else
                    tweet += "Active Markets:\n\n";

                orders.ForEach(am =>
                {
                    tweet += "\n$" + am.Market.Maker + " / $" + am.Market.Taker + ": " + am.Count;
                });

                tweets.Add(tweet);
            }

            return tweets;
        }

        public async Task<string> ComposeCompletedOrderTweet(ElapsedTime timeInterval)
        {
            var total = _dxDataRepository.GetTotalCompletedOrdersByElapsedTime(timeInterval);

            string tweet = "Completed Orders:\n\n";

            foreach (var coin in total.Keys)
            {
                tweet += "$" + coin + ": " + total[coin] + "\n";
            }

            return tweet;
        }

        public async Task<List<string>> ComposeVolumePerCoinTweets(ElapsedTime timeInterval)
        {
            var total = _dxDataRepository.GetTotalVolumeAndTradesByCoinAndElapsedTime(timeInterval);

            var childrenTweets = new List<string>();
            foreach (var coin in total.Keys)
            {
                var totalCoin = total[coin];
                if (totalCoin.NumberOfTrades > 0)
                {
                    string tweet = "Trading Volume $" + coin + ":\n\n";

                    tweet += "$USD: $" + totalCoin.USD.ToString("N2", CultureInfo.InvariantCulture) + "\n";
                    tweet += "$BTC: " + totalCoin.BTC.ToString("N3", CultureInfo.InvariantCulture) + " BTC\n";
                    tweet += "$BLOCK: " + totalCoin.BLOCK.ToString("N3", CultureInfo.InvariantCulture) + " BLOCK\n";

                    if (!units.Contains(coin))
                    {
                        tweet += "$" + coin + ": " + totalCoin.CustomCoin.ToString("N3", CultureInfo.InvariantCulture) + " " + coin + "\n";
                    }

                    tweet += "\n\nNumber of Trades: " + totalCoin.NumberOfTrades;

                    childrenTweets.Add(tweet);
                }
            }
            return childrenTweets;
        }
        public async Task<string> ComposeTotalVolumeTweet(ElapsedTime timeInterval)
        {
            var total = _dxDataRepository.GetTotalVolumeAndTradesByElapsedTime(timeInterval);

            string tweet = "1 Week @BlockDXExchange Statistics (" + DateTime.Now.ToUniversalTime().ToString("MMMM d yyyy") + " UTC)"
                + "\n\nTotal Trading Volume:"
                + "\n\n";

            if (total.NumberOfTrades.Equals(0))
            {
                throw new Exception("No 1 week volume on the BlockDX.");
            }

            tweet += "$USD: $" + total.USD.ToString("N2", CultureInfo.InvariantCulture) + "\n";
            tweet += "$BTC: " + total.BTC.ToString("N3", CultureInfo.InvariantCulture) + " BTC\n";
            tweet += "$BLOCK: " + total.BLOCK.ToString("N3", CultureInfo.InvariantCulture) + " BLOCK\n";

            tweet += "\nNumber of Trades: " + total.NumberOfTrades;

            return tweet;
        }

        public string ComposeMoreDetailsTweet()
        {
            string tweet = string.Empty;

            tweet += "\n\nMore live statistics below 👇";

            tweet += "\n\nOfficial: https://blockdx.com/orders/";
            tweet += "\n\nCommunity: https://blockdx.co/orders";

            return tweet;
        }
    }
}
