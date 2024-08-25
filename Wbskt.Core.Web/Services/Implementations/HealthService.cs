using System.Net.WebSockets;

namespace Wbskt.Core.Web.Services.Implementations
{
    // todo implement main logic
    public class HealthService : IHealthService
    {
        private readonly ILogger<HealthService> logger;

        public HealthService(ILogger<HealthService> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Listen(WebSocket webSocket)
        {
            await webSocket.ReadAsync();
        }
    }
}
