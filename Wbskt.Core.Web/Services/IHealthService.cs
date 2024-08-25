using System.Net.WebSockets;

namespace Wbskt.Core.Web.Services
{
    public interface IHealthService
    {
        Task Listen(WebSocket webSocket);
    }
}