using DXStats.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace DXStats.Services
{
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;

        const double interval = 300000; // 5 min

        private int counter = 0;

        private System.Timers.Timer _timer;

        public TimedHostedService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Dx Bot Service running.");

            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _publishService = scope.ServiceProvider.GetRequiredService<IPublishService>();

                    _publishService.Publish();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                throw;
            }


            counter = 0;

            Console.WriteLine("Starting fetchign trading data");

            _timer = new System.Timers.Timer(interval);
            _timer.Elapsed += new ElapsedEventHandler(GetTradingData);
            _timer.Enabled = true;

            return Task.CompletedTask;
        }

        private async void GetTradingData(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Block delta: " + counter);

            using (var scope = _scopeFactory.CreateScope())
            {
                var _dxDataRepository = scope.ServiceProvider.GetRequiredService<IDxDataRepository>();
                var _dxDataService = scope.ServiceProvider.GetRequiredService<IDxDataService>();
                var _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                // 1 block per minute. 5 blocks

                var tradingData = await _dxDataService.GetTradingData(5);
                _dxDataRepository.AddTradingData(tradingData);


                _dxDataRepository.AddCoinStatistics(tradingData);

                _unitOfWork.Complete();
            }

            counter++;

            if (counter == 2016) // 1440 blocks/minutes in a day. 7 days: 1440*7. (1440*7)/5 = 2016 * 5 minutes
            {

                using (var scope = _scopeFactory.CreateScope())
                {
                    var _publishService = scope.ServiceProvider.GetRequiredService<IPublishService>();

                    _publishService.Publish();
                }
                counter = 0;
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
