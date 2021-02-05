namespace HighchartsExportServer
{
    public static class Configuration
    {
        private const char URLS_DELIMITER = ';';

        public static string ParseUrl(this string str) => str.Split(URLS_DELIMITER)[0];
        
        public static int ParseIntOrDefault(this string str, int defaultValue) =>
            int.TryParse(str, out int result) ? result : defaultValue;

        public static readonly (string Key, string Default) HostUrl = ("HostUrl", "http://localhost:5000");
        public static readonly (string Key, string Default) PlatformIndicator = ("PlatformIndicator", "win32");
        public static readonly (string Key, int Default) JsRuntimeValidationInterval = ("JsRuntimeValidationInterval", 20);
        public static readonly (string Key, int Default) JsFunctionTimeout = ("JsFunctionTimeout", 3000);
    }
}