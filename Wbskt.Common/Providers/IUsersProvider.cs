using Wbskt.Common.Contracts;

namespace Wbskt.Common.Providers;

public interface IUsersProvider
{
    int FindByEmailId(string emailId);

    User? GetByEmailId(string emailId);

    User? GetById(int id);

    int Insert(User user);
}
