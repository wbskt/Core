using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IClientProvider
{
    ClientConnection GetClientConnectionById(int clientId);

    int FindClientIdByClientUniqueId(Guid clientUniqueId);

    IReadOnlyCollection<ClientConnection> GetClientConnectionsBySubscriberId(Guid channelSubscriberId);

    int AddOrUpdateClientConnection(ClientConnection clientConnection);

    void InvalidateToken(int clientId);
}
