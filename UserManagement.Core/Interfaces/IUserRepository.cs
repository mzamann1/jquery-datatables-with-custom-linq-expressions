using UserManagement.Core.Entities;

namespace UserManagement.Core.Interfaces
{
    public interface IUserRepository<T> where T : class
    {
        List<User> GetUsersList();
    }
}
