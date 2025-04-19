using Wbskt.Common;
using Wbskt.Common.Contracts;

namespace Wbskt.Core.Web.Services;

public interface IAuthService
{
    string GenerateToken(User userData);

    bool ValidatePassword(UserLoginRequest loginRequest);

    User RegisterUser(UserRegistrationRequest request);
}
