using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using OzonPriceChecker;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DataGatheringWindowsServices
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _consoleLogger;
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly AppSecret _appSecret;

        public Worker(ILogger<Worker> logger, AppSecret appSecret)
        {
            _consoleLogger = logger;
            _appSecret = appSecret;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _consoleLogger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                _consoleLogger.LogInformation("EMail: " + _appSecret.EmailLogin);
                _logger.Trace("Worker running at: {time}", DateTimeOffset.Now);

                var ozonPriceCheckerSecret = new OzonPriceCheckerSecret()
                {
                    EmailLogin = _appSecret.EmailLogin,
                    EmailPassword = _appSecret.EmailPassword,
                    SQliteConnectionString = _appSecret.SQliteConnectionString
                };

                var ozonPriceChecker = new OzonPriceChecker.OzonPriceChecker(ozonPriceCheckerSecret);
                var task = Task.Run(() => { return ozonPriceChecker.RunAsync(); });
                task.Wait();

                if (task.Result)
                {
                    _consoleLogger.LogInformation($"Task {nameof(OzonPriceChecker)} success!");
                }
                else
                {
                    _consoleLogger.LogError($"Task {nameof(OzonPriceChecker)} failed!");
                }

                _consoleLogger.LogInformation("Worker finish at: {time}. Now sleep.", DateTimeOffset.Now);
                _logger.Trace("Worker finish at: {time}. Now sleep.", DateTimeOffset.Now);
                var oneHourDelay = 3_600_000;
                for (int i = 0; i < 4; i++)
                {
                    await Task.Delay(oneHourDelay, stoppingToken);
                }
            }
        }
    }
}