using Microsoft.AspNetCore.Mvc;
using Wbskt.Common.Contracts;
using Wbskt.Core.Service.Services;

namespace Wbskt.Core.Service.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UsersController(ILogger<UsersController> logger, IUsersService usersService, IAuthService authService) : ControllerBase
{
    private readonly ILogger<UsersController> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IUsersService usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    private readonly IAuthService authService = authService ?? throw new ArgumentNullException(nameof(authService));

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
        if (string.IsNullOrWhiteSpace(request.UserName))
        {
            request.UserName = request.EmailId;
        }

        // todo: verify email is unique
        User user = authService.RegisterUser(request);
        // string token = authService.GenerateToken(user);
        return Ok("User created. Please login");
    }
}
