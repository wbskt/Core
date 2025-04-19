using Wbskt.Common;
using Wbskt.Common.Contracts;

namespace Wbskt.Core.Web.Services;

public interface IUsersService
{
    User GetUserById(int userId);

    User GetUserByEmailId(string emailId);

    User AddUser(User user);
}