using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Extensions;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChannelsController(ILogger<ChannelsController> logger, IChannelsService channelsService, IClientService clientService, IServerInfoService serverInfoService) : ControllerBase
{
    private readonly ILogger<ChannelsController> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IChannelsService channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
    private readonly IClientService clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
    private readonly IServerInfoService serverInfoService = serverInfoService ?? throw new ArgumentNullException(nameof(serverInfoService));

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
            logger.LogWarning("channel secret does not match the subscriptionId {channelSubscriberId}", request.ChannelSubscriberId);
            return Forbid("unauthorized");
        }

        // TODO: reuse the token if client did not use the previous token. (socket conn failure)
        string clientToken = clientService.AddClientConnection(request);
        return Ok(clientToken);
    }

    [HttpGet("{publisherId:guid}/dispatch")]
    [Authorize(AuthenticationSchemes = Constants.AuthSchemes.UserScheme)]
    public async Task<IActionResult> Dispatch(Guid publisherId)
    {
        var payload = new ClientPayload()
        {
            Data = "null"
        };
        return await Dispatch(publisherId, payload);
    }

    [HttpPost("{publisherId:guid}/dispatch")]
    [Authorize(AuthenticationSchemes = Constants.AuthSchemes.UserScheme)]
    public async Task<IActionResult> Dispatch(Guid publisherId, ClientPayload payload)
    {
        await serverInfoService.DispatchPayload(publisherId, payload);
        return Ok();
    }
}
