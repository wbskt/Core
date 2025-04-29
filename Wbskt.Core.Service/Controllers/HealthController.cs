using Microsoft.AspNetCore.Mvc;
using Wbskt.Common.Contracts;

namespace Wbskt.Core.Service.Controllers;

[Route("")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    [HttpHead]
    public IActionResult Ping()
    {
        var test = new ClientPayload()
        {
            Data = "Default payload"
        };
        return new JsonResult(test);
    }
}
