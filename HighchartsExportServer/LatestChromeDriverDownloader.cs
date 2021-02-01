using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HighchartsExportServer
{
    public class LatestChromeDriverDownloader : IChromeDriverDownloader
    {
        private const string LATEST_VERSION_KEY_URL = @"https://chromedriver.storage.googleapis.com/LATEST_RELEASE";
        private const string CHROME_DRIVER_STORAGE_URL = @"https://chromedriver.storage.googleapis.com/";
        private const string CONTENTS_ELEMENT_NAME = "Contents";
        private const string KEY_ELEMENT_NAME = "Key";
        private const string OUTPUT_FILENAME = "chromedriver.exe";

        private readonly HttpClient _httpClient;
        private readonly string _windowsVersionIndicator;

        public LatestChromeDriverDownloader(string windowsVersionIndicator)
        {
            _httpClient = new HttpClient();
            _windowsVersionIndicator = windowsVersionIndicator;
        }

        public LatestChromeDriverDownloader() : this("win32")
        {
        }

        public async Task DownloadAsync()
        {
            Task<string> latestChromeDriverKeyTask = _httpClient.GetStringAsync(LATEST_VERSION_KEY_URL);
            Task<XDocument> chromeDriverStorageTask = GetChromeDriverStorage();

            string latestChromeDriverUrl =
                GetLatestChromeDriverUrl(await chromeDriverStorageTask, await latestChromeDriverKeyTask);

            await DownloadLatestChromeDriver(latestChromeDriverUrl);
        }

        public void Dispose() => _httpClient.Dispose();

        private async Task<XDocument> GetChromeDriverStorage()
        {
            await using Stream httpContentStream = await _httpClient.GetStreamAsync(CHROME_DRIVER_STORAGE_URL);
            return await XDocument.LoadAsync(httpContentStream, LoadOptions.None, default);
        }

        private string GetLatestChromeDriverUrl(XDocument chromeDriverStorage, string latestChromeDriverKey)
        {
            XNamespace xmlNamespace = chromeDriverStorage.Root!.GetDefaultNamespace();

            string latestChromeDriverRoute = chromeDriverStorage.Root!
                .Elements($"{{{xmlNamespace}}}{CONTENTS_ELEMENT_NAME}")
                .Select(e => e.Element($"{{{xmlNamespace}}}{KEY_ELEMENT_NAME}")!.Value)
                .Where(s => s.Contains(latestChromeDriverKey, StringComparison.OrdinalIgnoreCase))
                .SingleOrDefault(s => s.Contains(_windowsVersionIndicator, StringComparison.OrdinalIgnoreCase));

            return CHROME_DRIVER_STORAGE_URL + latestChromeDriverRoute;
        }

        private async Task DownloadLatestChromeDriver(string latestChromeDriverUrl)
        {
            await using Stream httpContentStream = await _httpClient.GetStreamAsync(latestChromeDriverUrl);
            using var chromeDriverZipArchive = new ZipArchive(httpContentStream, ZipArchiveMode.Read);
            
            string outputPath = Path.Combine(Configuration.ChromeDriverDirectory, OUTPUT_FILENAME);
            chromeDriverZipArchive.Entries.First().ExtractToFile(outputPath, true);
        }
    }
}