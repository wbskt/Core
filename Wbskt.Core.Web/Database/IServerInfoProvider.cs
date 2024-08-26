using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Database
{
    public interface IServerInfoProvider
    {
        IReadOnlyCollection<ServerInfo> GetAll();
    }
}
