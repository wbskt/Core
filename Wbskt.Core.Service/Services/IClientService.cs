using Wbskt.Common;
using Wbskt.Common.Contracts;

namespace Wbskt.Core.Service.Services;

public interface IClientService
{
    string AddClientConnection(ClientConnectionRequest req);
}
