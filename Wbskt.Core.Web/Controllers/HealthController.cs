using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "Server")]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> logger;
        private readonly IHealthService healthService;

        public HealthController(ILogger<HealthController> logger, IHealthService healthService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.healthService = healthService ?? throw new ArgumentNullException(nameof(healthService));
        }

        [HttpPost]
        public async Task ConnectSocketServerAsync()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                logger.LogInformation("[Connection] accepted connection from \"{host}\"", HttpContext.Connection.RemoteIpAddress);

                var (_, message) = await webSocket.ReadAsync();
                logger.LogInformation("[Message] received message: \"{message}\"", message);

                try
                {
                    await healthService.Listen(webSocket);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                    logger.LogTrace(new EventId(0), ex, ex.Message);
                }
                finally
                {
                    webSocket.Dispose();
                    logger.LogInformation("[Connection] disposed connection");
                }
            }
            else
            {
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            }
        }
    }
}
