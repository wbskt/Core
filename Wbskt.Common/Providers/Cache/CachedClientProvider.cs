using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers.Cache
{
    internal sealed class CachedClientProvider(ILogger<CachedClientProvider> logger, IClientProvider clientProvider) : IClientProvider
    {
        private static readonly string ServerType = Environment.GetEnvironmentVariable(nameof(ServerType)) ?? Constants.ServerType.CoreServer;

        private readonly List<ClientConnection> channels = [];
        private readonly object _lock = new();

        // ((ClientProvider)clientProvider).RegisterSqlDependency(OnDatabaseChange);

        public int AddOrUpdateClientConnection(ClientConnection clientConnection)
        {
            return clientProvider.AddOrUpdateClientConnection(clientConnection);
        }

        public int FindClientIdByClientUniqueId(Guid clientUniqueId)
        {
            return clientProvider.FindClientIdByClientUniqueId(clientUniqueId);
        }

        public ClientConnection GetClientConnectionById(int clientId)
        {
            return clientProvider.GetClientConnectionById(clientId);
        }

        public IReadOnlyCollection<ClientConnection> GetClientConnectionsBySubscriberId(Guid channelSubscriberId)
        {
            return clientProvider.GetClientConnectionsBySubscriberId(channelSubscriberId);
        }

        public void InvalidateToken(int clientId)
        {
            clientProvider.InvalidateToken(clientId);
        }

        public IReadOnlyCollection<ClientConnection> GetClientConnectionsByIds(IEnumerable<int> clientIds)
        {
            return clientProvider.GetClientConnectionsByIds(clientIds);
        }

        private void OnDatabaseChange(object sender, SqlNotificationEventArgs e)
        {
            logger.LogInformation("database change detected: {Info}", e.Info);

            // RefreshCache();
            // ((ClientProvider)clientProvider).RegisterSqlDependency(OnDatabaseChange); // re-register after change
        }
    }
}
