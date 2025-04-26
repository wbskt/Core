using Wbskt.Common;
using Wbskt.Common.Contracts;

namespace Wbskt.Core.Web.Services;

public interface IClientService
{
    string AddClientConnection(ClientConnectionRequest req);
}
