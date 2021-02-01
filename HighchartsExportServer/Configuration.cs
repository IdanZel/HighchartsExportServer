using System.IO;
using Microsoft.Extensions.Configuration;

namespace HighchartsExportServer
{
    public static class Configuration
    {
        private const string URLS_CONFIGURATION_KEY = "ASPNETCORE_URLS";
        private const char URLS_DELIMITER = ';';
        
        public static string HostUrl { get; private set; }

        public static string ChromeDriverDirectory { get; private set; }
        
        public static int JsRuntimeValidationInterval { get; private set; }
        
        public static int JsFunctionTimeout { get; private set; }
        
        public static void Initialize(IConfiguration configuration)
        {
            HostUrl = configuration.GetHostUrl() ?? "http://localhost:5000";
            ChromeDriverDirectory = configuration[nameof(ChromeDriverDirectory)] ?? Path.GetTempPath();
            JsRuntimeValidationInterval = configuration[nameof(JsRuntimeValidationInterval)]?.ToInt() ?? 20;
            JsFunctionTimeout = configuration[nameof(JsFunctionTimeout)]?.ToInt() ?? 3000;
        }

        private static string GetHostUrl(this IConfiguration configuration) =>
            configuration[URLS_CONFIGURATION_KEY]?.Split(URLS_DELIMITER)[0];

        private static int ToInt(this string str) => int.Parse(str);
    }
}