using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IClientProvider
{
    int FindByClientNameUserId(string clientName, int userId);

    int FindByClientUniqueId(Guid clientUniqueId);

    IReadOnlyCollection<ClientConnection> GetAllByChannelId(int channelId);

    IReadOnlyCollection<ClientConnection> GetAllByClientIds(int[] clientIds);

    ClientConnection? GetByClientId(int clientId);

    int Upsert(ClientConnection clientConnection);
}
