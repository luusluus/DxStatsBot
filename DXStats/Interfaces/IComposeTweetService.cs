using DXStats.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DXStats.Interfaces
{
    public interface IComposeTweetService
    {
        Task<List<string>> ComposeOrdersAndActiveMarkets();
        Task<string> ComposeCompletedOrderTweet(ElapsedTime elapsedTime);
        Task<List<string>> ComposeVolumePerCoinTweets(ElapsedTime elapsedTime);
        Task<string> ComposeTotalVolumeTweet(ElapsedTime elapsedTime);
        string ComposeMoreDetailsTweet();
    }
}
