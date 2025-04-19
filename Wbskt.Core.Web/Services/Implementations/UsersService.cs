using Wbskt.Common;
using Wbskt.Common.Contracts;
using Wbskt.Common.Providers;

namespace Wbskt.Core.Web.Services.Implementations;

public class UsersService(ILogger<UsersService> logger, IUsersProvider usersProvider) : IUsersService
{
    private readonly ILogger<UsersService> logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IUsersProvider usersProvider = usersProvider ?? throw new ArgumentNullException(nameof(usersProvider));

    public User AddUser(User user)
    {
        user.UserId = usersProvider.AddUser(user);
        return user;
    }

    public User GetUserByEmailId(string emailId)
    {
        return usersProvider.GetUserByEmailId(emailId);
    }

    public User GetUserById(int userId)
    {
        return usersProvider.GetUserById(userId);
    }
}
