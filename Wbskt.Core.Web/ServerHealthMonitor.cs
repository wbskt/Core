using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Wbskt.Common;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web;

public class ServerHealthMonitor
{
    private Timer timer;
    private readonly string token;
    private readonly IConfiguration configuration;
    private readonly ILogger<ServerHealthMonitor> logger;
    private readonly IServerInfoService serverInfoService;

    public ServerHealthMonitor(ILogger<ServerHealthMonitor> logger, IServerInfoService serverInfoService, IConfiguration configuration)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));
        this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        token = CreateCoreServerToken();
        timer = new Timer(Ping, null, 0, 10 * 1000);
    }

    private async void Ping(object? state)
    {
        var servers = serverInfoService.GetAll();
        var header = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, token);
        var tasks = new List<Task>();
        var handler = new HttpClientHandler()
        {
            // todo: only for development
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        foreach (var server in servers)
        {
            logger.LogDebug("checking health of socket server: {ss}", server.Address);
            var task = Task.Run(async () => {
                var httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri($"https://{server.Address}"),
                    DefaultRequestHeaders = { Authorization = header}
                };
                HttpResponseMessage? result = null;
                try
                {
                    result = await httpClient.GetAsync("ping");
                }
                catch(Exception ex)
                {
                    logger.LogDebug("cannot reach server:{baseAddress}", httpClient.BaseAddress);
                }

                // only update if the status changed i.e. Active or NotActive
                if ((result?.IsSuccessStatusCode ?? false) != server.Active)
                {
                    server.Active = result?.IsSuccessStatusCode ?? false;
                    logger.LogInformation("health of socket server: {ss} - {active}", server.Address, server.Active);
                    serverInfoService.UpdateServerStatus(server.ServerId, result?.IsSuccessStatusCode ?? false);
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    private string CreateCoreServerToken()
    {
        var tokenHandler = new JsonWebTokenHandler();
        var configurationKey = configuration[Constants.JwtKeyNames.CoreServerTokenKey];

        var key = Encoding.UTF8.GetBytes(configurationKey!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(Constants.Claims.CoreServer, Guid.NewGuid().ToString())
            }),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256),
        };

        return tokenHandler.CreateToken(tokenDescriptor);
    }
}
