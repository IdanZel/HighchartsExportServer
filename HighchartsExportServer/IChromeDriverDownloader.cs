using System;
using System.Threading.Tasks;

namespace HighchartsExportServer
{
    public interface IChromeDriverDownloader : IDisposable
    {
        Task DownloadAsync();
    }
}