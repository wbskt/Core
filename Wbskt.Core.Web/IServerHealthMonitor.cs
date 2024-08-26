
namespace Wbskt.Core.Web
{
    public interface IServerHealthMonitor
    {
        int GetAvailableServerId();
        Task Ping(CancellationToken ct);
    }
}