namespace Wbskt.Core.Web.Services
{
    public interface IServerInfoService
    {
        IReadOnlyCollection<ServerInfo> GetAll();
        
        ServerInfo GetServerById(int id);

        void UpdateServerStatus(int id, bool active);

        int GetAvailableServerId();
    }
}
