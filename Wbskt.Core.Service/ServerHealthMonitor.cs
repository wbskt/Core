using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Wbskt.Core.Service.Services;

namespace Wbskt.Core.Service;

public class ServerHealthMonitor(ILogger<ServerHealthMonitor> logger, IServerInfoService serverInfoService, IAuthService authService)
{
    private readonly string token = authService.CreateCoreServerToken();
    private readonly ILogger<ServerHealthMonitor> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IServerInfoService serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));

    public async Task CheckHealthAsync(CancellationToken ct)
    {
        var servers = serverInfoService.GetAll();
        var header = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);

        var tasks = servers.Select(server => Task.Run(async () =>
            {
                logger.LogDebug("checking health of socket server: {ss}", server.Address);
                var httpClient = new HttpClient { BaseAddress = new Uri($"http://{server.Address}"), DefaultRequestHeaders = { Authorization = header } };
                HttpResponseMessage? result = null;
                try
                {
                    result = await httpClient.GetAsync("ping", ct);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "cannot reach server:{baseAddress}, error: {error}", httpClient.BaseAddress, ex.Message);
                }

                // only update if the status changed i.e. Active or NotActive
                if ((result?.IsSuccessStatusCode ?? false) != server.Active)
                {
                    server.Active = result?.IsSuccessStatusCode ?? false;
                    logger.LogInformation("socket server: {ss} - {active}", server.Address, server.Active);
                    serverInfoService.UpdateServerStatus(server.ServerId, server.Active);
                }
                else
                {
                    logger.LogDebug("response from:{host} is: {resp}", httpClient.BaseAddress, result?.ReasonPhrase ?? "exception while requesting (see above log)");
                }
            }, ct))
            .ToList();

        await Task.WhenAll(tasks);
    }
}
