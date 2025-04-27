using Wbskt.Common;
using Wbskt.Common.Contracts;

namespace Wbskt.Core.Service.Services;

public interface IServerInfoService
{
    IReadOnlyCollection<ServerInfo> GetAll();

    ServerInfo GetServerById(int id);

    void UpdateServerStatus(int id, bool active);

    int GetAvailableServerId();

    Task DispatchPayload(Guid publisherId, ClientPayload payload);

    void MapAllChannels();
}
