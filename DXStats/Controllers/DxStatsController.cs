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
        public DxStatsController(
            IDxDataRepository dxDataRepository
        )
        {
            _dxDataRepository = dxDataRepository;
        }

        [HttpGet("[action]")]
        public IActionResult GetTotalVolumeAndTrades(TimeInterval timeInterval)
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
        public IActionResult GetTotalVolumeAndTradesByCoin(TimeInterval timeInterval)
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
        public IActionResult GetTotalCompletedOrders(TimeInterval timeInterval)
        {
            var completedOrders = _dxDataRepository.GetTotalCompletedOrders(timeInterval);

            return Ok(completedOrders);
        }

        [HttpGet("[action]")]
        public IActionResult GetVolumeAndTradeCountByPeriod(Period period)
        {
            return Ok(_dxDataRepository.GetVolumeAndTradeCountByPeriod(period));
        }

        [HttpGet("[action]")]
        public IActionResult GetVolumeAndTradeCountByPeriodAndCoin(Period period, string coin)
        {
            var coins = _dxDataRepository.GetCoins().Select(c => c.Id).ToList();

            if (!coins.Contains(coin))
                return BadRequest("Coin not listed");

            return Ok(_dxDataRepository.GetVolumeAndTradeCountByPeriodAndCoin(period, coin));
        }
    }
}
