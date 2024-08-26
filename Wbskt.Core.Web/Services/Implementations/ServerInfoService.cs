
using Wbskt.Core.Web.Database;

namespace Wbskt.Core.Web.Services.Implementations
{
    public class ServerInfoService : IServerInfoService
    {
        private IDictionary<int, ServerInfo> allServers;
        private readonly ILogger<ServerInfoService> logger;
        private readonly IServerInfoProvider serverInfoProvider;

        public ServerInfoService(ILogger<ServerInfoService> logger, IServerInfoProvider serverInfoProvider)
        {
            allServers = new Dictionary<int, ServerInfo>();
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.serverInfoProvider = serverInfoProvider ?? throw new ArgumentNullException(nameof(serverInfoProvider));
        }
        
        public IReadOnlyCollection<ServerInfo> GetAll()
        {
            var servers = serverInfoProvider.GetAll();
            allServers = servers.ToDictionary(s => s.ServerId, s => s);
            return servers;
        }

        public ServerInfo GetServerById(int id)
        {
            if (allServers.ContainsKey(id))
            {
                return allServers[id];
            }
            else 
            {
                throw new ArgumentException();
            }
        }

        public void UpdateServerStatus(bool active)
        {
            throw new NotImplementedException();
        }
    }
}
