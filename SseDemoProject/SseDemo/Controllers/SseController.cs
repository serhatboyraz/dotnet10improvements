using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace SseDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SseController : ControllerBase
    {
        [HttpGet("time")]
        public async Task GetCurrentTime(CancellationToken cancellationToken)
        {
            Response.Headers.Append("Content-Type", "text/event-stream");
            Response.Headers.Append("Cache-Control", "no-cache");
            Response.Headers.Append("Connection", "keep-alive");

            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    var message = $"data: {currentTime}\n\n";
                    var bytes = Encoding.UTF8.GetBytes(message);

                    await Response.Body.WriteAsync(bytes, cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);

                    // Send time update every second
                    await Task.Delay(1000, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                // Client disconnected
            }
        }
    }
}
