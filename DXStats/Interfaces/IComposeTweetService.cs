using DXStats.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DXStats.Interfaces
{
    public interface IComposeTweetService
    {
        Task<List<string>> ComposeOrdersAndActiveMarkets();
        string ComposeCompletedOrderTweet(ElapsedTime elapsedTime);
        List<string> ComposeVolumePerCoinTweets(ElapsedTime elapsedTime);
        string ComposeTotalVolumeTweet(ElapsedTime elapsedTime);
        string ComposeMoreDetailsTweet();
    }
}
