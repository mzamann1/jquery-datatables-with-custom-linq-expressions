using JqueryDatatablePractice.Constants;

namespace JqueryDatatablePractice.Interfaces
{
    public interface IOrdering
    {
        IQueryable<T> GetOrderedData<T>(IQueryable<T> query, string name, OrderByType orderByType);
    }
}
