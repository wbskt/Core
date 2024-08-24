using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChannelsController : ControllerBase
    {
        private readonly ILogger<ChannelsController> logger;
        private readonly IChannelsService channelsService;

        public ChannelsController(ILogger<ChannelsController> logger, IChannelsService channelsService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.channelsService = channelsService ?? throw new ArgumentNullException(nameof(channelsService));
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var userId = User.GetUserId();
            IEnumerable<ChannelDetails> details = channelsService.GetChannelsForUser(userId);
            return Ok(details);
        }

        [HttpPost()]    
        public IActionResult CreateChannel(Channel channel)
        {
            channel.UserId = User.GetUserId();
            ChannelDetails details = channelsService.CreateChannel(channel);
            return Ok(details);
        }
    }
}
