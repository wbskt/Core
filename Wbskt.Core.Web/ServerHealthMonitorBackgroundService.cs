namespace Wbskt.Core.Web;

public class ServerHealthMonitorBackgroundService(ServerHealthMonitor monitor, ILogger<ServerHealthMonitorBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Server Health Monitor Background Service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            await monitor.CheckHealthAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // check every 10 seconds (example)
        }

        logger.LogInformation("Server Health Monitor Background Service is stopping.");
    }
}
