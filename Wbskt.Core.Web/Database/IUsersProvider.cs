namespace Wbskt.Core.Web.Database
{
    public interface IUsersProvider
    {
        int AddUser(UserData user);

        UserData GetUserDataByEmailId(string emailId);

        UserData GetUserDataById(int id);
    }
}
