using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IUsersProvider
{
    int AddUser(User user);

    User GetUserByEmailId(string emailId);

    User GetUserById(int id);

    int FindUserIdByEmailId(string emailId);
}
