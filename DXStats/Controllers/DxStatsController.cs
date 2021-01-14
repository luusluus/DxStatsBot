using DXStats.Controllers.Dtos;
using DXStats.Enums;
using DXStats.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        public IActionResult GetTotalVolumeAndTrades(ElapsedTime elapsedTime)
        {
            var total = _dxDataRepository.GetTotalVolumeAndTradesByElapsedTime(elapsedTime);

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
        public IActionResult GetTotalVolumeAndTradesByCoin(ElapsedTime elapsedTime)
        {
            var total = _dxDataRepository.GetTotalVolumeAndTradesByCoinAndElapsedTime(elapsedTime);

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
        public IActionResult GetTotalCompletedOrders(ElapsedTime elapsedTime)
        {
            var completedOrders = _dxDataRepository.GetTotalCompletedOrdersByElapsedTime(elapsedTime);

            return Ok(completedOrders);
        }

        [HttpGet("[action]")]
        public IActionResult GetVolumeAndTradeCountByElapsedTime(ElapsedTime elapsedTime)
        {
            if (elapsedTime.Equals(ElapsedTime.FiveMinutes) || elapsedTime.Equals(ElapsedTime.FifteenMinutes) || elapsedTime.Equals(ElapsedTime.Hour) || elapsedTime.Equals(ElapsedTime.TwoHours))
                return BadRequest("Lower boundary is one hour");

            var volumeAndTradeCount = _dxDataRepository.GetVolumeAndTradeCountByElapsedTime(elapsedTime);

            return Ok(volumeAndTradeCount.Select(v => new TotalVolumeAndTradeCountIntervalDto
            {
                Timestamp = v.Timestamp,
                TradeCount = v.TradeCount,
                Volumes = new Dictionary<string, decimal>
                {
                    { nameof(v.BTC), v.BTC  },
                    { nameof(v.USD), v.USD  },
                    { nameof(v.BLOCK), v.BLOCK  },
                }
            }));
        }

        [HttpGet("[action]")]
        public IActionResult GetVolumeAndTradeCountByElapsedTimeAndCoin(ElapsedTime elapsedTime, string coin)
        {
            if (elapsedTime.Equals(ElapsedTime.FiveMinutes) || elapsedTime.Equals(ElapsedTime.FifteenMinutes) || elapsedTime.Equals(ElapsedTime.Hour) || elapsedTime.Equals(ElapsedTime.TwoHours))
                return BadRequest("Lower boundary is one hour");

            var coins = _dxDataRepository.GetCoins().Select(c => c.Id).ToList();

            if (!coins.Contains(coin))
                return BadRequest("Coin not listed");

            var volumeAndTradeCount = _dxDataRepository.GetVolumeAndTradeCountByElapsedTimeAndCoin(elapsedTime, coin);

            return Ok(volumeAndTradeCount.Select(v => new TotalVolumeAndTradeCountIntervalDto
            {
                Timestamp = v.Timestamp,
                TradeCount = v.TradeCount,
                Volumes = new Dictionary<string, decimal>
                {
                    { nameof(v.BTC), v.BTC  },
                    { nameof(v.USD), v.USD  },
                    { nameof(v.BLOCK), v.BLOCK  },
                    { coin, v.CustomCoin  }
                }
            }));
        }
    }
}
