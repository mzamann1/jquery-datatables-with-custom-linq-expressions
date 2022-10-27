using System.Data.Common;
using System.Linq;
using LINQExtensions.Models;
using LINQExtensions.Interfaces;
using Zaman.Library.LINQExtensions.Constants;
using LINQExtensions.Common;
using Zaman.Library.LINQExtensions.Extensions;
using LINQExtensions.Models.ViewModels.JQueryDatatables;
using System.Security.AccessControl;
using Zaman.Library.LINQExtensions.Helpers;

namespace LINQExtensions.Services
{
    public class UserService : IUserService
    {
        public List<User> GetUsers()
        {
            return CommonData.GetUsersList();
        }

        public IQueryable<T> GetPaginatedData<T>(IQueryable<T> query, JQueryDtRequest dt)
        {
            return query.Skip(dt.Start).Take(dt.Length);
        }

        public IQueryable<T> GetOrderedData<T>(IQueryable<T> query, JQueryDtRequest dt)
        {

            if (dt.Order.Length > 0)
                return query.OrderBy(dt.Columns[dt.Order[0].Column].Name, dt.Order[0].Dir == "asc" ? OrderByType.Ascending : OrderByType.Descending);
            return query;
        }

        public IQueryable<T> GetFilteredData<T>(IQueryable<T> query, JQueryDtRequest dt)
        {

            if (string.IsNullOrWhiteSpace(dt.Search.Value) && dt.Columns.All(x => string.IsNullOrWhiteSpace(x.Search.Value)) && dt.searchBuilder == null)
            {
                return query;
            }

            var searchableCols = dt.Columns.Where(x => x.Searchable);

            int searchableCount = searchableCols.Count();

            var sourceType = typeof(T);

            var properties = sourceType.GetProperties().Join(searchableCols, prop1 => prop1.Name, prop2 => prop2.Name, (prop1, prop2) => new
            {
                PropertyType = prop1.PropertyType,
                Name = prop1.Name,
                SearchValue = prop2.Search.Value
            });


            if (dt.searchBuilder != null)
            {
                foreach (var item in dt.SearchBuilder.Criteria)
                {
                    query = query.Where(item.Data, item.Value[0], (MethodType)Enum.Parse(typeof(MethodType), item.Condition));
                }
            }


            //if (searchableCount > 0 && searchableCols.Any(x => !string.IsNullOrWhiteSpace(x.Search.Value)))
            //{
            //    foreach (var prop in properties)
            //    {
            //        if (string.IsNullOrWhiteSpace(prop.SearchValue))
            //        {
            //            continue;
            //        }

            //        if (Helper.IsNumericType(prop.PropertyType))
            //        {
            //            bool valueIsNumeric = decimal.TryParse(prop.SearchValue, out decimal numericValue);
            //            if (valueIsNumeric)
            //            {
            //                query = query.Where(prop.Name, numericValue);

            //            }
            //        }
            //        else
            //        {
            //            query = query.Where(prop.Name, prop.SearchValue);
            //        }

            //    }

            //}

            //if (searchableCount > 0 && !string.IsNullOrWhiteSpace(dt.Search.Value))
            //{
            //    var colNames = searchableCols.Select(x => x.Name);

            //    foreach (var prop in properties)
            //    {
            //        if (Helper.IsNumericType(prop.PropertyType))
            //        {
            //            bool valueIsNumeric = decimal.TryParse(dt.Search.Value, out decimal numericValue);
            //            if (valueIsNumeric)
            //            {
            //                query = query.Where(colNames, numericValue);
            //            }
            //        }
            //        else
            //        {
            //            query = query.Where(colNames, dt.Search.Value, MethodType.Contains);
            //        }

            //    };
            //}

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

            return query;
        }
    }
}
