using JqueryDatatablePractice.Constants;
using JqueryDatatablePractice.Models;

namespace JqueryDatatablePractice.Interfaces
{
    public interface IUserService : IPagination, IOrdering,IFiltering
    {
        List<User> GetUsers();
    }
}
