namespace Wbskt.Core.Web.Services
{
    public interface IUsersService
    {
        User GetUserByName(string username);

        User GetUserByEmail(string email);

        User AddUser(User user);
    }
}
