namespace Wbskt.Core.Web.Services
{
    public interface IAuthService
    {
        string GenerateToken(User userData);

        string GenerateSalt();

        bool ValidatePassword(UserLoginRequest loginRequest);

        User RegisterUser(UserRegistrationRequest request);
    }
}
