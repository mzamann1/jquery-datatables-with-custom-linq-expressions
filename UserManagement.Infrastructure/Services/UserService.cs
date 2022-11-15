using UserManagement.Core.Constants;
using UserManagement.Core.Entities;
using UserManagement.Core.Extensions;
using UserManagement.Core.Interfaces;
using UserManagement.Core.Models.RequestModels.JQuery;
using IUserService = UserManagement.Core.Interfaces.IUserService;

namespace UserManagement.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository<User> _userRepository;

        public UserService(IUserRepository<User> _userRepository)
        {
            this._userRepository = _userRepository;
        }

        public List<User> FetchUsers() => _userRepository.GetUsersList();

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

            if (string.IsNullOrWhiteSpace(dt.Search.Value) && dt.Columns.All(x => string.IsNullOrWhiteSpace(x.Search.Value)) && dt.SearchBuilder == null)
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


            if (dt.SearchBuilder != null)
            {
                foreach (var item in dt.SearchBuilder.Criteria)
                {
                    if (item.Type == "string")
                    {
                        query = query.Where(item.Data, item.Value[0], (MethodType)Enum.Parse(typeof(MethodType), item.Condition));
                    }
                    else if (item.Type == "num")
                    {
                        var condition = (ConditionalOperatorType)Enum.Parse(typeof(ConditionalOperatorType), item.Condition);

                        bool isFirstNumeric = decimal.TryParse(item.Value1 ?? "", out decimal numericValue); //value from first input box

                        if (condition == ConditionalOperatorType.Between || condition == ConditionalOperatorType.NotBetween)
                        {
                            //value from second input box
                            bool isSecondNumeric = decimal.TryParse(item.Value2 ?? "", out decimal secondNumeric);

                            /*
                             * if both inputs are empty
                             */

                            if (!isFirstNumeric && !isSecondNumeric)
                            {
                                continue;
                            }

                            /*
                             *
                             * If only first inout is provided (From - To) => from range
                             *
                             * weird logic behind is if user types in first(from) input and second one is empty, then first input value  being stored in 2nd element of the array which is value2
                             *
                             */

                            if (!isFirstNumeric && isSecondNumeric)
                            {
                                query = query.Where(item.Data, secondNumeric, condition == ConditionalOperatorType.Between ? ConditionalOperatorType.GreaterThan : ConditionalOperatorType.LessThan);
                            }
                            else
                            {
                                query = query.Where(item.Data, isFirstNumeric ? numericValue : null, isSecondNumeric ? secondNumeric : null, condition);
                            }
                        }
                        else
                        {
                            if (string.IsNullOrWhiteSpace(item.Value1))
                            {
                                continue;
                            }

                            query = query.Where(item.Data, numericValue, item.Value2, condition);

                        }
                    }
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
