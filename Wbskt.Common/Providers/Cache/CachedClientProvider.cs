using Wbskt.Common.Contracts;
using Wbskt.Common.Providers.Implementations;

namespace Wbskt.Common.Providers.Cache
{
    internal sealed class CachedClientProvider(ClientProvider clientProvider) : IClientProvider
    {
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

        public IReadOnlyCollection<ClientConnection> GetClientConnectionsByIds(int[] clientIds)
        {
            return clientProvider.GetClientConnectionsByIds(clientIds);
        }

        public bool Exists(string reqClientName, Guid reqChannelSubscriberId)
        {
            return clientProvider.Exists(reqClientName, reqChannelSubscriberId);
        }
    }
}
