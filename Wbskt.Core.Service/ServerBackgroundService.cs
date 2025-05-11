﻿namespace Wbskt.Core.Service;

public class ServerBackgroundService(ILogger<ServerBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("server background Service is starting...");
        await Task.Run(() =>
        {
            while (true) { }
        }, stoppingToken);
    }
}
