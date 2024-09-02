using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web
{
    public class ServerHealthMonitor
    {
        private readonly ILogger<ServerHealthMonitor> logger;
        private readonly IServerInfoService serverInfoService;
        private Timer timer;

        public ServerHealthMonitor(ILogger<ServerHealthMonitor> logger, IServerInfoService serverInfoService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));
            timer = new Timer(Ping, null, 0, 10 * 1000);
        }

        private async void Ping(object? state)
        {
            var servers = serverInfoService.GetAll();

            var tasks = new List<Task>();
            var handler = new HttpClientHandler()
            {
                // todo: only for development
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator 
            };
            foreach (var server in servers)
            {
                var task = Task.Run(async () => { 
                    var httpClient = new HttpClient(handler)
                    {
                        BaseAddress = new Uri($"https://{server.Address}"),
                    };
                    HttpResponseMessage? result = null; 
                    try
                    {
                        result = await httpClient.GetAsync("ping");
                    }
                    catch(Exception ex)
                    {
                        logger.LogInformation($"cannot reach server:{httpClient.BaseAddress}");
                    }

                    if (result?.IsSuccessStatusCode ?? false != server.Active)
                    {
                        server.Active = result?.IsSuccessStatusCode ?? false;
                        serverInfoService.UpdateServerStatus(server.ServerId, result?.IsSuccessStatusCode ?? false);
                    }
                });

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }
    }
}
