using System.Data.Common;
using System.Linq;
using LINQExtensions.Models;
using LINQExtensions.Interfaces;
using LINQExtensions.Constants;
using LINQExtensions.Common;
using LINQExtensions.Extensions;
using LINQExtensions.Models.ViewModels.JQueryDatatables;

namespace LINQExtensions.Services
{
    public class UserService : IUserService
    {
        public List<User> GetUsers()
        {
            return CommonData.GetUsersList();
        }

        public IQueryable<T> GetPaginatedData<T>(IQueryable<T> query, DtRequestModel dt)
        {
            return query.Skip(dt.Start).Take(dt.Length);
        }

        public IQueryable<T> GetOrderedData<T>(IQueryable<T> query, DtRequestModel dt)
        {

            if (dt.Order.Length > 0)
                return query.OrderBy(dt.Columns[dt.Order[0].Column].Name, dt.Order[0].Dir == "asc" ? OrderByType.Ascending : OrderByType.Descending);
            return query;
        }

        public IQueryable<T> GetFilteredData<T>(IQueryable<T> query, DtRequestModel dt)
        {

            return query.Where(dt);

            //foreach (var column in dt.Columns)
            //{
            //    if (!string.IsNullOrWhiteSpace(column.Search.Value))
            //    {
            //        bool valueIsNumeric = decimal.TryParse(column.Search.Value, out decimal numericValue);
            //        if (valueIsNumeric)
            //        {
            //            query = query.Where(column.Name, numericValue, ConditionalOperatorType.Equals);
            //        }
            //        else
            //        {
            //            query = query.Where(column.Name, column.Search.Value, MethodType.Contains);
            //        }
            //    }
            //}


            //var searchableColumns = dt.Columns.Where(x => x.Searchable);



            //foreach (var col in searchableColumns)
            //{
            //    var property = typeof(T).GetProperty(col.Name);

            //    if (property != null)
            //    {
            //        if (Helper.IsNumericType(property.PropertyType) )
            //        {

            //        }
            //        else
            //        {

            //        }
            //    }

            //}


            //bool valueIsNumeric = decimal.TryParse(value, out decimal numericValue);
            //if (valueIsNumeric)
            //{
            //    query = query.Where(searchableColumns, numericValue);
            //}
            //else
            //{
            //    query = query.Where(searchableColumns, value, MethodType.Contains);
            //}

            //return query;
        }
    }
}
