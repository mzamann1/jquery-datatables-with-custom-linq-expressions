
namespace UserManagement.Core.Interfaces
{
    public interface IOrdering
    {
        IQueryable<T> GetOrderedData<T>(IQueryable<T> query, IGridRequest dt);
    }
}
