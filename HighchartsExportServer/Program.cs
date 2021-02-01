using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HighchartsExportServer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>())
                .ConfigureServices(services => services
                    .AddSingleton<IChromeDriverDownloader, LatestChromeDriverDownloader>()
                    .AddHostedService<WebDriverService>()
                    .AddSignalR(options => options.MaximumReceiveMessageSize = null))
                .ConfigureLogging(builder =>
                    builder.AddSimpleConsole(options =>
                    {
                        options.TimestampFormat = "[dd/MM/yyyy HH:mm:ss.fff UTC] ";
                        options.UseUtcTimestamp = true;
                    }))
                .ConfigureAppConfiguration(Configure)
                .UseConsoleLifetime()
                .Build()
                .RunAsync();
        }

        private static void Configure(IConfigurationBuilder builder) => new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()).AddEnvironmentVariables().Build();
    }
}