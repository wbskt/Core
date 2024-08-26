
namespace Wbskt.Core.Web
{
    public interface IServerHealthMonitor
    {
        Task Ping(CancellationToken ct);
    }
}