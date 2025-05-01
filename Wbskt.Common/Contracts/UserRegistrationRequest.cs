namespace Wbskt.Common.Contracts;

public class UserRegistrationRequest : UserLoginRequest
{
    public required string UserName { get; set; }
}
