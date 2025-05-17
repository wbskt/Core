using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers.Cache;

internal sealed class CachedClientProvider(IClientProvider clientProvider) : IClientProvider
{
    public int FindByClientNameUserId(string clientName, int userId)
    {
        throw new NotImplementedException();
    }

    public int FindByClientUniqueId(Guid clientUniqueId)
    {
        throw new NotImplementedException();
    }

    public IReadOnlyCollection<ClientConnection> GetAll()
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

    public void AddClientChannel(int clientId, int channelId)
    {
        throw new NotImplementedException();
    }

    public void RemoveClientChannel(int clientId, int channelId)
    {
        throw new NotImplementedException();
    }

    public void SetClientChannels(int clientId, int[] channelIds)
    {
        throw new NotImplementedException();
    }
}
