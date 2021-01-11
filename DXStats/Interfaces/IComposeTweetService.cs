using DXStats.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DXStats.Interfaces
{
    public interface IComposeTweetService
    {
        Task<List<string>> ComposeOrdersAndActiveMarkets();
        Task<string> ComposeCompletedOrderTweet(TimeInterval timeInterval);
        Task<List<string>> ComposeVolumePerCoinTweets(TimeInterval timeInterval);
        Task<string> ComposeTotalVolumeTweet(TimeInterval timeInterval);
        string ComposeMoreDetailsTweet();
    }
}
