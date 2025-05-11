using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;

namespace Wbskt.Core.Service.Controllers;

[Route("")]
[ApiController]
public class HealthController(ILogger<HealthController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Ping()
    {
        var test = new ClientPayload();
        return new JsonResult(test);
    }

    [HttpGet("/ws")]
    public async Task Get()
    {
        if (HttpContext.WebSockets.IsWebSocketRequest)
        {
            try
            {
                using var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
                await Echo(webSocket);
            }
            catch (Exception ex)
            {
                logger.LogError("unexpected error occured while maintaining connection to socket server: {error}", ex.Message);
                HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
        }
        else
        {
            HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
    }

    private static async Task Echo(WebSocket webSocket)
    {
        var result = await webSocket.ReadAsync();

        while (!result.ReceiveResult.CloseStatus.HasValue)
        {
            result = await webSocket.ReadAsync();
        }

        await webSocket.CloseAsync(result.ReceiveResult.CloseStatus.Value, result.ReceiveResult.CloseStatusDescription, CancellationToken.None);
    }
}
