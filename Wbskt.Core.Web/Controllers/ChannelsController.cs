using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelsController(ILogger<ChannelsController> logger, IChannelsService channelsService, IClientService clientService) : ControllerBase
{
    private readonly ILogger<ChannelsController> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IChannelsService channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
    private readonly IClientService clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));

    [HttpGet]
    [Authorize(AuthenticationSchemes = Constants.AuthSchemes.UserScheme)]
    public IActionResult GetAll()
    {
        var userId = User.GetUserId();
        IEnumerable<ChannelDetails> details = channelsService.GetChannelsForUser(userId);
        return Ok(details);
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = Constants.AuthSchemes.UserScheme)]
    public IActionResult CreateChannel(ChannelRequest channel)
    {
        channel.UserId = User.GetUserId();
        ChannelDetails details = channelsService.CreateChannel(channel);
        return Ok(details);
    }

    [HttpPost("client")]
    [AllowAnonymous]
    public IActionResult SubscribeToChannel(ClientConnectionRequest request)
    {
        // todo: cache this channel in the channel service.
        if (!channelsService.VerifyChannel(request.ChannelSubscriberId, request.ChannelSecret))
        {
            return Forbid("unauthorized");
        }

        string clientToken = clientService.AddClientConnection(request);
        return Ok(clientToken);
    }
}
