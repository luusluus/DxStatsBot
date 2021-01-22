using DXStats.Enums;
using DXStats.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;

namespace DXStats.Controllers
{
    [ApiController]
    [Route("api/dxstats")]
    public class DxStatsController : ControllerBase
    {
        private readonly IDxDataRepository _dxDataRepository;
        private readonly IPublishService _publishService;

        public DxStatsController(
            IDxDataRepository dxDataRepository,
            IPublishService publishService
        )
        {
            _dxDataRepository = dxDataRepository;
            _publishService = publishService;
        }

        [HttpGet("[action]")]
        public IActionResult GetTradingData(ElapsedTime elapsedTime)
        {
            return Ok(_dxDataRepository.GetTradingData(elapsedTime));
        }

        [HttpGet("[action]")]
        public IActionResult GetTotalVolumeAndTrades(ElapsedTime elapsedTime)
        {
            return Ok(_dxDataRepository.GetTotalVolumeAndTradesByElapsedTime(elapsedTime));
        }

        [HttpGet("[action]")]
        public IActionResult GetTotalVolumeAndTradesByCoin(ElapsedTime elapsedTime)
        {
            return Ok(_dxDataRepository.GetTotalVolumeAndTradesByCoinAndElapsedTime(elapsedTime));
        }

        [HttpGet("[action]")]
        public IActionResult GetTotalCompletedOrdersByElapsedTime(ElapsedTime elapsedTime)
        {
            return Ok(_dxDataRepository.GetTotalCompletedOrdersByElapsedTime(elapsedTime));
        }

        [HttpGet("[action]")]
        public IActionResult GetVolumeAndTradeCountByElapsedTime(ElapsedTime elapsedTime)
        {
            throw new NotImplementedException();
            //if (elapsedTime.Equals(ElapsedTime.FiveMinutes) || elapsedTime.Equals(ElapsedTime.FifteenMinutes) || elapsedTime.Equals(ElapsedTime.Hour) || elapsedTime.Equals(ElapsedTime.TwoHours))
            //    return BadRequest("Lower boundary is one hour");

            //var volumeAndTradeCount = _dxDataRepository.GetVolumeAndTradeCountByElapsedTime(elapsedTime);

            //return Ok(volumeAndTradeCount.Select(v => new TotalVolumeAndTradeCountIntervalDto
            //{
            //    Timestamp = v.Timestamp,
            //    TradeCount = v.TradeCount,
            //    Volumes = new Dictionary<string, decimal>
            //    {
            //        { nameof(v.BTC), v.BTC  },
            //        { nameof(v.USD), v.USD  },
            //        { nameof(v.BLOCK), v.BLOCK  },
            //    }
            //}));
        }

        [HttpGet("[action]")]
        public IActionResult GetVolumeAndTradeCountByElapsedTimeAndCoin(ElapsedTime elapsedTime, string coin)
        {
            throw new NotImplementedException();
            //if (elapsedTime.Equals(ElapsedTime.FiveMinutes) || elapsedTime.Equals(ElapsedTime.FifteenMinutes) || elapsedTime.Equals(ElapsedTime.Hour) || elapsedTime.Equals(ElapsedTime.TwoHours))
            //    return BadRequest("Lower boundary is one hour");

            //var coins = _dxDataRepository.GetCoins().Select(c => c.Id).ToList();

            //if (!coins.Contains(coin))
            //    return BadRequest("Coin not listed");

            //var volumeAndTradeCount = _dxDataRepository.GetVolumeAndTradeCountByElapsedTimeAndCoin(elapsedTime, coin);

            //return Ok(volumeAndTradeCount.Select(v => new TotalVolumeAndTradeCountIntervalDto
            //{
            //    Timestamp = v.Timestamp,
            //    TradeCount = v.TradeCount,
            //    Volumes = new Dictionary<string, decimal>
            //    {
            //        { nameof(v.BTC), v.BTC  },
            //        { nameof(v.USD), v.USD  },
            //        { nameof(v.BLOCK), v.BLOCK  },
            //        { coin.ToUpper(), v.CustomCoin  }
            //    }
            //}));
        }
    }
}
