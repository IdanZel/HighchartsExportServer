using System;
using System.Threading;
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

        public static async Task<T> InvokeJsFunctionAsync<T>(string function, int jsFunctionTimeout, 
            int jsRuntimeValidationInterval, params object[] parameters)
        {
            CancellationToken cancellationToken = new CancellationTokenSource(jsFunctionTimeout).Token;
            
            while (_jsRuntime == null)
            {
                await Task.Delay(jsRuntimeValidationInterval, cancellationToken);
                cancellationToken.ThrowIfCancellationRequested();
            }

            return await _jsRuntime.InvokeAsync<T>(function, TimeSpan.FromMilliseconds(jsFunctionTimeout), parameters);
        }
    }
}