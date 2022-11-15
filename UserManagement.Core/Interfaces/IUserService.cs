using UserManagement.Core.Entities;
using UserManagement.Core.Models.RequestModels.JQuery;

namespace UserManagement.Core.Interfaces
{
    public interface IUserService
    {
        public List<User> FetchUsers();

        public IQueryable<T> GetPaginatedData<T>(IQueryable<T> query, JQueryDtRequest dt);

        public IQueryable<T> GetOrderedData<T>(IQueryable<T> query, JQueryDtRequest dt);

        public IQueryable<T> GetFilteredData<T>(IQueryable<T> query, JQueryDtRequest dt);
    }
}


