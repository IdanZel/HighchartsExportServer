using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace HighchartsExportServer
{
    public class WebDriverService : IHostedService, IDisposable
    {
        private const string CHROME_HEADLESS_ARGUMENT = "--headless";
        private const string INITIALIZE_JS_RUNTIME_ROUTE = "/InitializeJsRuntime";

        private readonly ILogger<WebDriverService> _logger;
        private readonly IChromeDriverDownloader _chromeDriverDownloader;
        private IWebDriver _webDriver;

        public WebDriverService(ILogger<WebDriverService> logger, IChromeDriverDownloader chromeDriverDownloader)
        {
            _logger = logger;
            _chromeDriverDownloader = chromeDriverDownloader;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Downloading latest chrome driver.");
            
            await _chromeDriverDownloader.DownloadAsync();
            
            _logger.LogInformation("Initializing Selenium Chrome web driver (headless).");

            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument(CHROME_HEADLESS_ARGUMENT);
            _webDriver = new ChromeDriver(ChromeDriverService.CreateDefaultService(Configuration.ChromeDriverDirectory), chromeOptions);

            _logger.LogInformation("Requesting InitializeJsRuntime.");

            _webDriver.Navigate().GoToUrl(Configuration.HostUrl + INITIALIZE_JS_RUNTIME_ROUTE);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Disposing Selenium Chrome web driver.");

            _webDriver.Dispose();
            return Task.CompletedTask;
        }

        public void Dispose() => _chromeDriverDownloader.Dispose();
    }
}