using Wbskt.Common.Contracts;
using Wbskt.Common.Providers.Implementations;

namespace Wbskt.Common.Providers.Cache
{
    internal sealed class CachedClientProvider(ClientProvider clientProvider) : IClientProvider
    {
        public int FindByClientNameUserId(string clientName, int userId)
        {
            throw new NotImplementedException();
        }

        public int FindByClientUniqueId(Guid clientUniqueId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ClientConnection> GetAllByChannelId(int channelId)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ClientConnection> GetAllByClientIds(int[] clientIds)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<ClientConnection> GetAllByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public ClientConnection? GetByClientId(int clientId)
        {
            throw new NotImplementedException();
        }

        public int Upsert(ClientConnection clientConnection)
        {
            throw new NotImplementedException();
        }
    }
}
