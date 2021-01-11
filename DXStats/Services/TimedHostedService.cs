using Discord;
using Discord.WebSocket;
using DXStats.Configuration;
using DXStats.Enums;
using DXStats.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Tweetinvi;

namespace DXStats.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        static readonly List<string> units = new List<string>()
            {
                "BLOCK",
                "BTC",
                "USD"
            };

        //const double interval = 1000 * 60 * 60 * 24 * 7;

        const double interval = 1000 * 60 * 15; // 15 minutes

        private int counter = 0;

        private System.Timers.Timer _timer;

        private readonly IServiceScopeFactory _scopeFactory;

        private readonly DiscordSocketClient _discordSocketClient;
        IOptions<DiscordCredentials> _discordCredentials;

        public TimedHostedService(
            IServiceScopeFactory scopeFactory,
            DiscordSocketClient discord,
            IOptions<DiscordCredentials> discordCredentials
            )
        {
            _scopeFactory = scopeFactory;
            _discordSocketClient = discord;
            _discordCredentials = discordCredentials;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Dx Bot Service running.");


            _timer = new System.Timers.Timer(interval);
            _timer.Elapsed += new ElapsedEventHandler(CreateSnapshot);
            _timer.Enabled = true;

            return Task.CompletedTask;
        }



        private async void CreateSnapshot(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Snapshot");

            using (var scope = _scopeFactory.CreateScope())
            {
                var _dxDataRepository = scope.ServiceProvider.GetRequiredService<IDxDataRepository>();
                var _dxDataService = scope.ServiceProvider.GetRequiredService<IDxDataService>();
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                _dxDataRepository.AddDailySnapshot(
                    await _dxDataService.GetTotalCompletedOrders(),
                    await _dxDataService.GetTotalVolumePerCoin(string.Join(",", units))
                );

                _unitOfWork.Complete();
            }

            counter++;

            if (counter == 7) //TODO: publish each week, 15 minutes counter incremented.
            {
                //await Publish();
                counter = 0;
            }
        }
        private async Task Publish()
        {
            Console.WriteLine("Publish");
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var composeTweetService = scope.ServiceProvider.GetRequiredService<IComposeTweetService>();

                    var mainTweet = await composeTweetService.ComposeTotalVolumeTweet(TimeInterval.Week);
                    if (!string.IsNullOrEmpty(mainTweet))
                    {
                        Console.WriteLine(mainTweet);
                        var childrenTweets = await composeTweetService.ComposeVolumePerCoinTweets(TimeInterval.Week);

                        childrenTweets.ForEach(ct => Console.WriteLine(ct));

                        var completedOrdersTweet = await composeTweetService.ComposeCompletedOrderTweet(TimeInterval.Week);

                        Console.WriteLine(completedOrdersTweet);

                        var openOrdersTweets = await composeTweetService.ComposeOrdersAndActiveMarkets();

                        openOrdersTweets.ForEach(ct => Console.WriteLine(ct));

                        var detailsTweet = composeTweetService.ComposeMoreDetailsTweet();

                        Console.WriteLine(detailsTweet);

                        var parentTweet = Tweet.PublishTweet(mainTweet);

                        Tweetinvi.Models.ITweet prevTweet = parentTweet;
                        Tweetinvi.Models.ITweet currTweet;
                        foreach (var childTweet in childrenTweets)
                        {
                            currTweet = Tweet.PublishTweetInReplyTo(childTweet, prevTweet);
                            prevTweet = currTweet;
                        };

                        var completedOrdersPostedTweet = Tweet.PublishTweetInReplyTo(completedOrdersTweet, prevTweet);

                        prevTweet = completedOrdersPostedTweet;
                        foreach (var openOrderTweet in openOrdersTweets)
                        {
                            currTweet = Tweet.PublishTweetInReplyTo(openOrderTweet, prevTweet);
                            prevTweet = currTweet;
                        };


                        Tweet.PublishTweetInReplyTo(detailsTweet, prevTweet);

                        var channelId = Convert.ToUInt64(_discordCredentials.Value.ChannelId);

                        var discordChannel = _discordSocketClient.GetChannel(channelId) as IMessageChannel;
                        await discordChannel.SendMessageAsync("test");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Timed Hosted Service is stopping.");

            _timer.Enabled = false;

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
