namespace Wbskt.Core.Web.Services
{
    public interface IAuthService
    {
        string GenerateToken(User userData);

        string GenerateSalt();

        bool ValidatePasswordWithSalt(LoginRequest loginRequest);

        User RegisterNewPlayer(UserRegistrationRequest request);
    }
}
