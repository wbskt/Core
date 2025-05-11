using System.Net.WebSockets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;
using Wbskt.Core.Service.Services;

namespace Wbskt.Core.Service.Controllers;

[Route("")]
[ApiController]
public class HealthController(ILogger<HealthController> logger, IServerInfoService serverInfoService) : ControllerBase
{
    [HttpGet]
    public IActionResult Ping()
    {
        var test = new ClientPayload();
        return new JsonResult(test);
    }

    [HttpGet("/ws")]
    [Authorize(AuthenticationSchemes = Constants.AuthSchemes.SocketServerScheme)]
    public async Task Get()
    {
        var sid = User.GetSocketServerId();
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            try
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                serverInfoService.UpdateServerStatus(sid, true);
                await Echo(webSocket, Program.Cts.Token);
            }
            catch (Exception ex)
            {
                logger.LogError("unexpected error occured while maintaining connection to socket server: {error}", ex.Message);
            }
            finally
            {
                serverInfoService.UpdateServerStatus(sid, false);
            }
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private async Task Echo(WebSocket ws, CancellationToken ct)
    {
        ct.Register(() => CloseClientConnection(logger, ws).Wait(CancellationToken.None));
        var result = await ws.ReadAsync(CancellationToken.None);

        while (!result.ReceiveResult.CloseStatus.HasValue)
        {
            result = await ws.ReadAsync(CancellationToken.None);
        }

        if (ws.State is WebSocketState.Open or WebSocketState.CloseReceived or WebSocketState.CloseSent)
        {
            logger.LogInformation("closing connection ({closeStatus})", result.ReceiveResult.CloseStatusDescription);
            await ws.CloseAsync(result.ReceiveResult.CloseStatus.Value, result.ReceiveResult.CloseStatusDescription, CancellationToken.None);
        }
    }

    private static async Task CloseClientConnection(ILogger logger, WebSocket ws)
    {
        if (ws.State is WebSocketState.Open or WebSocketState.CloseReceived or WebSocketState.CloseSent)
        {
            logger.LogInformation("closing connection ({closeStatus})", "Closing connection (client initiated)");
            await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing connection (client initiated)", CancellationToken.None);
        }
    }
}
