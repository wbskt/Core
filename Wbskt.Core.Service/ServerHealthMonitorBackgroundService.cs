namespace Wbskt.Core.Service;

public class ServerHealthMonitorBackgroundService(ServerHealthMonitor monitor, ILogger<ServerHealthMonitorBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Server Health Monitor Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await monitor.CheckHealthAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError("error while checking health of socket servers: {error}", ex.Message);
                logger.LogTrace("error while checking health of socket servers: {error}", ex.ToString());
            }
        }

        logger.LogInformation("Server Health Monitor Background Service is stopping.");
    }
}
