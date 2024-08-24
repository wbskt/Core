using Wbskt.Core.Web.Services;

namespace Wbskt.Core.Web.Database
{
    public interface IUsersProvider
    {
        int AddUser(User user);

        User GetUserByEmailId(string emailId);

        User GetUserById(int id);
    }
}
