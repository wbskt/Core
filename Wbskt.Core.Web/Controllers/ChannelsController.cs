using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChannelsController : ControllerBase
    {
        private readonly ILogger<ChannelsController> logger;
        private readonly IChannelsService channelsService;
        private readonly IClientService clientService;

        public ChannelsController(ILogger<ChannelsController> logger, IChannelsService channelsService, IClientService clientService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
            this.clientService = clientService ?? throw new ArgumentNullException(nameof(clientService));
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAll()
        {
            var userId = User.GetUserId();
            IEnumerable<ChannelDetails> details = channelsService.GetChannelsForUser(userId);
            return Ok(details);
        }

        [HttpPost()]
        [Authorize]
        public IActionResult CreateChannel(Channel channel)
        {
            channel.UserId = User.GetUserId();
            ChannelDetails details = channelsService.CreateChannel(channel);
            return Ok(details);
        }

        [HttpPost("client")]
        public IActionResult GetConnectionToken(ClientConnectionRequest request)
        {
            /*
            clientname - subscriberid - clientid

            */
            string clientToken = clientService.RegisterClientConnection(request);

            return Ok(clientToken);
        }
    }
}
