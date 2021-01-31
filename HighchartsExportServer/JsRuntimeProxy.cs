using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace HighchartsExportServer
{
    public static class JsRuntimeProxy
    {
        private static IJSRuntime _jsRuntime;

        public static void SetJsRuntime(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public static async Task<T> InvokeJsFunctionAsync<T>(string function, params object[] parameters)
        {
            while (_jsRuntime == null)
            {
                await Task.Delay(Configuration.JsRuntimeValidationInterval);
            }

            int timeout = Configuration.JsFunctionTimeout;
            return await _jsRuntime.InvokeAsync<T>(function, TimeSpan.FromMilliseconds(timeout), parameters);
        }
    }
}