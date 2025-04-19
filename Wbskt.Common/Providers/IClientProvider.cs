using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IClientProvider
{
    ClientConnection GetClientConnectionById(int clientId);

    IReadOnlyCollection<ClientConnection> GetClientConnectionsBySubscriberId(Guid channelSubscriberId);

    int AddOrUpdateClientConnection(ClientConnection clientConnection);

    void InvalidateToken(int clientId);
}
