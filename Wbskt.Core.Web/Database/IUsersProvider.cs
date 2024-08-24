namespace Wbskt.Core.Web.Database
{
    public interface IUsersProvider
    {
        int AddUser(UserData user);

        UserData GetUserDataByName(string username);

        UserData GetUserDataById(int id);
    }
}
