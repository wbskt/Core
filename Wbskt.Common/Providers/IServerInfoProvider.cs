using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IServerInfoProvider
{
    IReadOnlyCollection<ServerInfo> GetAll();

    void UpdateServerStatus(int id, bool active);
    int RegisterServer(ServerInfo serverInfo);
}
