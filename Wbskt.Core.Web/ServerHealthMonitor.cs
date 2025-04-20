using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web;

public class ServerHealthMonitor
{
    private Timer timer;
    private readonly string token;
    private readonly ILogger<ServerHealthMonitor> logger;
    private readonly IServerInfoService serverInfoService;

    public ServerHealthMonitor(ILogger<ServerHealthMonitor> logger, IServerInfoService serverInfoService, IAuthService authService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));
        token = authService.CreateCoreServerToken();
        timer = new Timer(Ping, null, 0, 10 * 1000); // 10 seconds
    }

    private async void Ping(object? state)
    {
        var servers = serverInfoService.GetAll();
        var header = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        var handler = new HttpClientHandler()
        {
            // todo: only for development
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var tasks = servers.Select(server => Task.Run(async () =>
            {
                logger.LogDebug("checking health of socket server: {ss}", server.Address);
                var httpClient = new HttpClient(handler) { BaseAddress = new Uri($"https://{server.Address}"), DefaultRequestHeaders = { Authorization = header } };
                HttpResponseMessage? result = null;
                try
                {
                    result = await httpClient.GetAsync("ping");
                }
                catch (Exception ex)
                {
                    logger.LogWarning("cannot reach server:{baseAddress}, error: {error}", httpClient.BaseAddress, ex.Message);
                    logger.LogTrace("cannot reach server:{baseAddress}, error: {error}", httpClient.BaseAddress, ex.ToString());
                }

                // only update if the status changed i.e. Active or NotActive
                if ((result?.IsSuccessStatusCode ?? false) != server.Active)
                {
                    server.Active = result?.IsSuccessStatusCode ?? false;
                    logger.LogInformation("socket server: {ss} - {active}", server.Address, server.Active);
                    serverInfoService.UpdateServerStatus(server.ServerId, server.Active);
                }
            }))
            .ToList();

        await Task.WhenAll(tasks);
    }
}
