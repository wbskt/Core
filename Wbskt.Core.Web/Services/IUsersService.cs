namespace Wbskt.Core.Web.Services
{
    public interface IUsersService
    {
        User GetUserByName(string username);

        User GetUserByEmailId(string emailId);

        User AddUser(User user);
    }
}
