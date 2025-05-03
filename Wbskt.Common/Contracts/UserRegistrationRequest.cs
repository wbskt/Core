namespace Wbskt.Common.Contracts;

public class UserRegistrationRequest : UserLoginRequest
{
    public string UserName { get; set; } = string.Empty;
}
