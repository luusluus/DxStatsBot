using Discord.WebSocket;
using DXStats.Configuration;
using DXStats.Enums;
using DXStats.Interfaces;
using Microsoft.Extensions.Options;
using System;

namespace DXStats.Services
{
    public class PublishService : IPublishService
    {
        private readonly IComposeTweetService _composeTweetService;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IOptions<DiscordCredentials> _discordCredentials;

        public PublishService(
            IComposeTweetService composeTweetService,
            DiscordSocketClient discord,
            IOptions<DiscordCredentials> discordCredentials)
        {
            _composeTweetService = composeTweetService;
            _discordSocketClient = discord;
            _discordCredentials = discordCredentials;
        }
        public void Publish()
        {
            Console.WriteLine("Publish");
            try
            {
                var mainTweet = _composeTweetService.ComposeTotalVolumeTweet(ElapsedTime.Week);
                if (!string.IsNullOrEmpty(mainTweet))
                {
                    Console.WriteLine(mainTweet);
                    var childrenTweets = _composeTweetService.ComposeVolumePerCoinTweets(ElapsedTime.Week);

                    childrenTweets.ForEach(ct => Console.WriteLine(ct));

                    var completedOrdersTweet = _composeTweetService.ComposeCompletedOrderTweet(ElapsedTime.Week);

                    Console.WriteLine(completedOrdersTweet);

                    var openOrdersTweets = _composeTweetService.ComposeOrdersAndActiveMarkets();

                    openOrdersTweets.Result.ForEach(ct => Console.WriteLine(ct));

                    var detailsTweet = _composeTweetService.ComposeMoreDetailsTweet();

                    Console.WriteLine(detailsTweet);

                    //var parentTweet = Tweet.PublishTweet(mainTweet);

                    //Tweetinvi.Models.ITweet prevTweet = parentTweet;
                    //Tweetinvi.Models.ITweet currTweet;
                    //foreach (var childTweet in childrenTweets)
                    //{
                    //    currTweet = Tweet.PublishTweetInReplyTo(childTweet, prevTweet);
                    //    prevTweet = currTweet;
                    //};

                    //var completedOrdersPostedTweet = Tweet.PublishTweetInReplyTo(completedOrdersTweet, prevTweet);

                    //prevTweet = completedOrdersPostedTweet;
                    //foreach (var openOrderTweet in openOrdersTweets.Result)
                    //{
                    //    currTweet = Tweet.PublishTweetInReplyTo(openOrderTweet, prevTweet);
                    //    prevTweet = currTweet;
                    //};


                    //Tweet.PublishTweetInReplyTo(detailsTweet, prevTweet);

                    //var channelId = Convert.ToUInt64(_discordCredentials.Value.ChannelId);

                    //var discordChannel = _discordSocketClient.GetChannel(channelId) as IMessageChannel;
                    //discordChannel.SendMessageAsync(parentTweet.Url);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                throw ex;
            }
        }
    }
}
