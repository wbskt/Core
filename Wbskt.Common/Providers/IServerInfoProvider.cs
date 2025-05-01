using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IServerInfoProvider
{
    IReadOnlyCollection<ServerInfo> GetAllServerInfo();

    void UpdateServerStatus(int id, bool active);

    void UpdatePublicDomainName(int id, string publicDomainName);

    int RegisterServer(ServerInfo serverInfo);
}
