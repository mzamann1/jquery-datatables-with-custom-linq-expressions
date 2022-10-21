using LINQExtensions.Models;

namespace LINQExtensions.Interfaces
{
    public interface IUserService : IPagination, IOrdering, IFiltering
    {
        List<User> GetUsers();
    }
}
