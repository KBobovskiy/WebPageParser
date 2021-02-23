using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using System;

namespace DataGatheringWindowsServices
{
    public class Program
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.AddEnvironmentVariables(prefix: "WebParser_");
                })
            .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();

                    IConfiguration configuration = hostContext.Configuration;

                    AppSecret appSecret = configuration.GetSection(nameof(AppSecret)).Get<AppSecret>();
                    if (appSecret == null)
                    {
                        _logger.Error($"{nameof(appSecret)} is NULL!");
                        throw new NullReferenceException($"{nameof(appSecret)} is NULL!");
                    }

                    services.AddSingleton(appSecret);
                });
    }
}