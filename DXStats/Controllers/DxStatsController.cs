using Discord.WebSocket;
using DXStats.Configuration;
using DXStats.Controllers.Dtos;
using DXStats.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;

namespace DXStats.Controllers
{
    [ApiController]
    [Route("api/dxstats")]
    public class DxStatsController : ControllerBase
    {
        private readonly IDxDataRepository _dxDataRepository;
        private readonly IOptions<DiscordCredentials> _discordCreds;
        private readonly DiscordSocketClient _discordSocketClient;
        public DxStatsController(IDxDataRepository dxDataRepository, IOptions<DiscordCredentials> discordCreds, DiscordSocketClient discordSocketClient)
        {
            _dxDataRepository = dxDataRepository;
            _discordCreds = discordCreds;
            _discordSocketClient = discordSocketClient;
        }

        [HttpGet("[action]")]
        public IActionResult GetWeeklyStatsTotal()
        {
            var total = _dxDataRepository.GetTotalVolumeAndTradesByWeek();

            var dto = new TotalVolumeDto
            {
                NumberOfTrades = total.NumberOfTrades,
                BTC = total.BTC,
                USD = total.USD,
                BLOCK = total.BLOCK
            };
            return Ok(dto);
        }

        [HttpGet("[action]")]
        public IActionResult GetWeeklyStatsByCoin()
        {
            var total = _dxDataRepository.GetTotalVolumeAndTradesByWeekAndCoin();

            var dto = total.ToDictionary(x => x.Key, x => new TotalVolumePerCoinDto
            {
                BTC = x.Value.BTC,
                BLOCK = x.Value.BLOCK,
                USD = x.Value.USD,
                NumberOfTrades = x.Value.NumberOfTrades,
                CustomCoin = x.Value.CustomCoin
            });

            return Ok(dto);
        }

        [HttpGet("[action]")]
        public IActionResult GetWeeklyCompletedOrders()
        {
            var completedOrders = _dxDataRepository.GetCompletedOrdersByWeek();


            //var channelId = Convert.ToUInt64(_discordCreds.Value.ChannelId);


            //var discordChannel = _discordSocketClient.GetChannel(channelId) as IMessageChannel;
            //Task.Run(() => discordChannel.SendMessageAsync("Test"));

            return Ok(completedOrders);
        }
    }
}
