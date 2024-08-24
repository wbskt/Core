using Wbskt.Core.Web.Database;

namespace Wbskt.Core.Web.Services.Implementations
{
    public class UsersService : IUsersService
    {
        private readonly ILogger<UsersService> logger;
        private readonly IUsersProvider usersProvider;

        public UsersService(ILogger<UsersService> logger, IUsersProvider usersProvider)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.usersProvider = usersProvider ?? throw new ArgumentNullException(nameof(usersProvider));
        }

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
}
