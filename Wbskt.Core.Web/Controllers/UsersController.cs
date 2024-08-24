using Microsoft.AspNetCore.Mvc;
using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IUsersService usersService;
        private readonly IAuthService authService;

        public UsersController(ILogger<UsersController> logger, IUsersService usersService, IAuthService authService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        }

        [HttpPost("login")]
        public IActionResult UserLogin(UserLoginRequest request)
        {
            bool valid = authService.ValidatePassword(request);
            if (!valid)
            {
                return Forbid("credentials are incorrect");
            }

            User user = usersService.GetUserByEmailId(request.EmailId);
            string token = authService.GenerateToken(user);
            return Ok(token);
        }

        [HttpPost("register")]
        public IActionResult UserRegistration(UserRegistrationRequest request)
        {
            // todo: verify email is unique
            User user = authService.RegisterUser(request);
            string token = authService.GenerateToken(user);
            return Ok(token);
        }
    }
}
