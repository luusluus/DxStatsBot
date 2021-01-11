using DXStats.Controllers.Dtos;
using DXStats.Enums;
using DXStats.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DXStats.Controllers
{
    [ApiController]
    [Route("api/dxstats")]
    public class DxStatsController : ControllerBase
    {
        private readonly IDxDataRepository _dxDataRepository;
        //private readonly IOptions<DiscordCredentials> _discordCreds;
        //private readonly DiscordSocketClient _discordSocketClient;
        public DxStatsController(
            //IOptions<DiscordCredentials> discordCreds, 
            //DiscordSocketClient discordSocketClient,
            IDxDataRepository dxDataRepository
        )
        {
            _dxDataRepository = dxDataRepository;
            //_discordCreds = discordCreds;
            //_discordSocketClient = discordSocketClient;
        }

        [HttpGet("[action]")]
        public IActionResult GetWeeklyStatsTotal(TimeInterval timeInterval)
        {
            var total = _dxDataRepository.GetTotalVolumeAndTrades(timeInterval);

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
        public IActionResult GetWeeklyStatsByCoin(TimeInterval timeInterval)
        {
            var total = _dxDataRepository.GetTotalVolumeAndTradesByCoin(timeInterval);

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
        public IActionResult GetWeeklyCompletedOrders(TimeInterval timeInterval)
        {
            var completedOrders = _dxDataRepository.GetTotalCompletedOrders(timeInterval);

            return Ok(completedOrders);
        }
    }
}
