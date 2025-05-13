using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IClientProvider
{
    int FindByClientNameUserId(string clientName, int userId);

    int FindByClientUniqueId(Guid clientUniqueId);

    IReadOnlyCollection<ClientConnection> GetAll();

    IReadOnlyCollection<ClientConnection> GetAllByChannelId(int channelId);

    IReadOnlyCollection<ClientConnection> GetAllByClientIds(int[] clientIds);

    IReadOnlyCollection<ClientConnection> GetAllByUserId(int userId);

    ClientConnection? GetByClientId(int clientId);

    int Upsert(ClientConnection clientConnection);
}
