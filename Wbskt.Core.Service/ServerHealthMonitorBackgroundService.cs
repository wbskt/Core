using Wbskt.Core.Service.Services;

namespace Wbskt.Core.Service;

public class ServerHealthMonitorBackgroundService(IServerInfoService serverInfoService, ServerHealthMonitor monitor, ILogger<ServerHealthMonitorBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("server Health Monitor Background Service is starting.");
        var channelsMapped = false;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (channelsMapped == false)
                {
                    serverInfoService.MapAllChannels();
                    channelsMapped = true;
                }

                await monitor.CheckHealthAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "error while checking health of socket servers: {error}", ex.Message);
            }
            finally
            {
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }

        logger.LogInformation("server Health Monitor Background Service is stopping.");
    }
}
