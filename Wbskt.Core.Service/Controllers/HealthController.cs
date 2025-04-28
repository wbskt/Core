using Microsoft.AspNetCore.Mvc;
using Wbskt.Common.Contracts;

namespace Wbskt.Core.Service.Controllers;

[Route("")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Ping()
    {
        var test = new ClientPayload();
        return new JsonResult(test);
    }
}
