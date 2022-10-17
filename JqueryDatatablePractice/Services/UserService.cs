using JqueryDatatablePractice.Constants;
using JqueryDatatablePractice.Interfaces;
using JqueryDatatablePractice.Models;
using JqueryDatatablePractice.Extensions;
using JqueryDatatablePractice.Models.ViewModels;
using System.Data.Common;
using System.Linq;
using JqueryDatatablePractice.Common;

namespace JqueryDatatablePractice.Services
{
    public class UserService : IUserService
    {
        public List<User> GetUsers()
        {
            return CommonData.GetUsersList();
        }

        public IQueryable<T> GetPaginatedData<T>(IQueryable<T> query, int start, int end)
        {
            return query.Skip(start).Take(end);
        }

        public IQueryable<T> GetOrderedData<T>(IQueryable<T> query, string name, OrderByType orderByType)
        {
            return query.OrderBy(name, orderByType);
        }

        public IQueryable<T> GetFilteredData<T>(IQueryable<T> query, DtRequest dt, string value)
        {
            var searchableColumns = dt.Columns.Where(x => x.Searchable);

            bool valueIsNumeric = decimal.TryParse(value, out decimal numericValue);
            if (valueIsNumeric)
            {
                query = query.Where(searchableColumns, numericValue);
            }
            else
            {
                query = query.Where(searchableColumns, value, MethodType.Contains);
            }

            return query;
        }
    }
}
