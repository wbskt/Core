namespace Wbskt.Common.Contracts;

public class UserLoginRequest
{
    public required string EmailId { get; set; }

    public required string Password { get; set; }
}
