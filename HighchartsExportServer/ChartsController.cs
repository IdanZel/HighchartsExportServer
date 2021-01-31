using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HighchartsExportServer
{
    public class ChartsController : ControllerBase
    {
        private const string PNG_CONTENT_TYPE = "image/png";

        private static ILogger<ChartsController> _logger;
        
        public ChartsController(ILogger<ChartsController> logger)
        {
            _logger ??= logger;
        }
        
        [HttpPost]
        [Route("api/createChart")]
        public async Task<IActionResult> CreateChart([FromBody] object data, [FromQuery] int width)
        {
            DateTime executionStartTime = DateTime.Now;
            _logger.LogTrace($"Received CreateChart request. Width: {width}; Data: {data}");

            string svg;

            try
            {
                svg = await JsRuntimeProxy.InvokeJsFunctionAsync<string>("exportChart", data.ToString());
            }
            catch (TaskCanceledException e)
            {
                double totalExecutionTime = ExecutionTime(executionStartTime);
                _logger.LogError($"Chart export was cancelled after {totalExecutionTime}. " +
                                 $"Exception: {Environment.NewLine}{e}");
                return BadRequest($"Chart generation was timed-out after {totalExecutionTime} ms.");
            }

            if (string.IsNullOrEmpty(svg))
            {
                _logger.LogError("No data was returned from chart export.");
                return BadRequest("Chart generation has failed.");
            }

            _logger.LogTrace("Successfully generated chart SVG: " + svg);

            byte[] pngBytes;
            
            try
            {
                pngBytes = await ImageConversion.SvgToPng(width, svg);
            }
            catch (Exception e)
            {
                _logger.LogError("SVG to PNG conversion failed. Exception: " + Environment.NewLine + e);
                return BadRequest("SVG to PNG conversion failed.");
            }

            _logger.LogInformation($"Successfully generated chart PNG. Execution time: {ExecutionTime(executionStartTime)} ms");
            return File(pngBytes, PNG_CONTENT_TYPE);
        }

        private static double ExecutionTime(DateTime startTime) => (DateTime.Now - startTime).TotalMilliseconds;
    }
}