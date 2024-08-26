using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web
{
    public class ServerHealthMonitor : IServerHealthMonitor
    {
        private readonly ILogger<ServerHealthMonitor> logger;
        private readonly IChannelsService channelsService;
        private readonly IServerInfoService serverInfoService;

        private readonly HashSet<int> activeServers;

        public ServerHealthMonitor(ILogger<ServerHealthMonitor> logger, IChannelsService channelsService, IServerInfoService serverInfoService)
        {
            activeServers = new HashSet<int>();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
            this.serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));
        }

        public int GetAvailableServerId()
        {
            throw new NotImplementedException();
        }

        public async Task Ping(CancellationToken ct)
        {
            var servers = serverInfoService.GetAll();

            while (!ct.IsCancellationRequested)
            {
                foreach (var server in servers)
                {
                    var httpClient = new HttpClient
                    {
                        BaseAddress = new Uri($"https://{server.Address}"),
                    };

                    var result = await httpClient.GetAsync("ping");

                    if (result.IsSuccessStatusCode != server.Active)
                    {
                        server.Active = result.IsSuccessStatusCode;
                        serverInfoService.UpdateServerStatus(result.IsSuccessStatusCode);

                        if (server.Active)
                        {
                            activeServers.Add(server.ServerId);
                        }
                        else
                        {
                            activeServers.Remove(server.ServerId);
                        }
                    }
                }
                await Task.Delay(10 * 1000);
            }
        }
    }
}
