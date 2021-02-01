using System.Threading.Tasks;

namespace HighchartsExportServer
{
    public interface ISvgToPngConverter
    {
        Task<byte[]> ConvertAsync(int width, string svg);
    }
}