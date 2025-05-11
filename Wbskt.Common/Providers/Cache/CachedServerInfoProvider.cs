using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Wbskt.Common.Contracts;
using Wbskt.Common.Providers.Implementations;

namespace Wbskt.Common.Providers.Cache
{
    internal sealed class CachedServerInfoProvider : IServerInfoProvider
    {
        private static readonly string ServerType = Environment.GetEnvironmentVariable(nameof(ServerType)) ?? Constants.ServerType.CoreServer.ToString();

        private readonly List<ServerInfo> serverInfos = [];
        private readonly object @lock = new();
        private readonly ILogger<CachedServerInfoProvider> logger;
        private readonly ServerInfoProvider serverInfoProvider;

        public CachedServerInfoProvider(ILogger<CachedServerInfoProvider> logger, ServerInfoProvider serverInfoProvider)
        {
            this.logger = logger;
            this.serverInfoProvider = serverInfoProvider;

            serverInfoProvider.RegisterSqlDependency(OnDatabaseChange);
        }

        public IReadOnlyCollection<ServerInfo> GetAllSocketServerInfo()
        {
            var servers = GetAll();
            return servers.Where(s => s.Type == Constants.ServerType.SocketServer).ToArray();
        }

        public IReadOnlyCollection<ServerInfo> GetAllServerInfo()
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ServerInfo> GetAllCoreServerInfo()
        {
            var servers = GetAll();
            return servers.Where(s => s.Type == Constants.ServerType.CoreServer).ToArray();
        }

        public int RegisterServer(ServerInfo serverInfo)
        {
            if (ServerType == Constants.ServerType.CoreServer.ToString())
            {
                logger.LogError("core server cannot perform this operation: {operationName}", nameof(RegisterServer));
                return -1;
            }

            return serverInfoProvider.RegisterServer(serverInfo);
        }

        public void UpdatePublicDomainName(int id, string publicDomainName)
        {
            if (id <= 0)
            {
                logger.LogError("invalid id provided: {id}", id);
                throw new ArgumentException("invalid id provided", nameof(id));
            }

            if (ServerType == Constants.ServerType.CoreServer.ToString())
            {
                logger.LogError("core server cannot perform this operation: {operationName}", nameof(UpdatePublicDomainName));
                return;
            }

            serverInfoProvider.UpdatePublicDomainName(id, publicDomainName);
            lock (@lock)
            {
                var info = serverInfos.FirstOrDefault(s => s.ServerId == id);
                if (info != null)
                {
                    info.PublicDomainName = publicDomainName;
                }
            }
        }

        public void UpdateServerStatus(int id, bool active)
        {
            if (id <= 0)
            {
                logger.LogError("invalid id provided: {id}", id);
                throw new ArgumentException("invalid id provided", nameof(id));
            }

            serverInfoProvider.UpdateServerStatus(id, active);

            lock (@lock)
            {
                var info = serverInfos.FirstOrDefault(s => s.ServerId == id);
                if (info != null)
                {
                    info.Active = active;
                }
            }
        }

        private IReadOnlyCollection<ServerInfo> GetAll()
        {
            lock (@lock)
            {
                if (serverInfos.Count != 0)
                {
                    return [.. serverInfos]; // return a copy to prevent external mutation
                }

                var records = serverInfoProvider.GetAllServerInfo();
                serverInfos.AddRange(records);

                return [.. serverInfos]; // return a copy to prevent external mutation
            }
        }

        private void RefreshCache()
        {
            var records = serverInfoProvider.GetAllServerInfo();
            lock (@lock)
            {
                serverInfos.Clear();
                serverInfos.AddRange(records);
            }
        }

        private void OnDatabaseChange(object sender, SqlNotificationEventArgs e)
        {
            logger.LogInformation("database change detected: {Info}", e.Info);

            RefreshCache();
            serverInfoProvider.RegisterSqlDependency(OnDatabaseChange); // re-register after change
        }
    }
}
