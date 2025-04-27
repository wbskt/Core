namespace Wbskt.Core.Service.Services;

public class UserLoginRequest
{
    public required string EmailId { get; set; }

    public required string Password { get; set; }
}