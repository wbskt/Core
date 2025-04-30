using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IServerInfoProvider
{
    IReadOnlyCollection<ServerInfo> GetAll();
    void UpdateServerStatus(int id, bool active);
    void UpdateServerStatus(int id, string publicDomainName);
    int RegisterServer(ServerInfo serverInfo);
}
