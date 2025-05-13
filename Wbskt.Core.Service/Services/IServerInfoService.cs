using Wbskt.Common.Contracts;

namespace Wbskt.Core.Service.Services;

public interface IServerInfoService
{
    void UpdateServerStatus(int id, bool active);

    Task<bool> DispatchPayload(ClientPayload payload);
}
