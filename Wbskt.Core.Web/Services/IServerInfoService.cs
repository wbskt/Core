namespace Wbskt.Core.Web.Services
{
    public interface IServerInfoService
    {
        IReadOnlyCollection<ServerInfo> GetAll();
        ServerInfo GetServerById(int id);
        void UpdateServerStatus(bool active);
    }
}
