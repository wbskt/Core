using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IServerInfoProvider
{
    IReadOnlyCollection<ServerInfo> GetAll();

    int Insert(ServerInfo serverInfo);

    void UpdatePublicDomainName(int id, string publicDomainName);

    void UpdateServerStatus(int id, bool active);
}

public interface ICachedServerInfoProvider : IServerInfoProvider
{
    IReadOnlyCollection<ServerInfo> GetAllSocketServerInfo();

    IReadOnlyCollection<ServerInfo> GetAllCoreServerInfo();

    ServerInfo GetById(int serverId);
}
