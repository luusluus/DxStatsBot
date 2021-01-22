using DXStats.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;

namespace DXStats.Controllers
{
    [ApiController]
    [Route("api/pub")]
    public class PublishController : ControllerBase
    {
        private readonly IHostedService _timeHostedService;

        public PublishController(IHostedService timeHostedService)
        {
            _timeHostedService = timeHostedService as TimedHostedService;
        }

        [HttpPost("[action]")]
        public IActionResult PublishWeeklyStats()
        {
            try
            {
                _timeHostedService.StopAsync(new System.Threading.CancellationToken());
                _timeHostedService.StartAsync(new System.Threading.CancellationToken());
                return Ok("Published");
            }
            catch (Exception e)
            {
                return Ok("Not published: \n" + e.Message);
            }

        }
    }
}
