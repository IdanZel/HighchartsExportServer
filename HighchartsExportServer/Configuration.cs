using Microsoft.Extensions.Configuration;

namespace HighchartsExportServer
{
    public static class Configuration
    {
        private const string URLS_CONFIGURATION_KEY = "ASPNETCORE_URLS";
        private const char URLS_DELIMITER = ';';
        
        public static string HostUrl { get; private set; }
        
        public static int JsRuntimeValidationInterval { get; private set; }
        
        public static int JsFunctionTimeout { get; private set; }
        
        public static void Initialize(IConfiguration configuration)
        {
            HostUrl = configuration.GetHostUrl();
            JsRuntimeValidationInterval = int.Parse(configuration[nameof(JsRuntimeValidationInterval)]);
            JsFunctionTimeout = int.Parse(configuration[nameof(JsFunctionTimeout)]);
        }

        private static string GetHostUrl(this IConfiguration configuration) =>
            configuration[URLS_CONFIGURATION_KEY].Split(URLS_DELIMITER)[0];
    }
}