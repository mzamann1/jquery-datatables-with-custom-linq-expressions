namespace UserManagement.Core.Interfaces
{
    public interface IPagination
    {
        IQueryable<T> GetPaginatedData<T>(IQueryable<T> query, IGridRequest dt);
    }
}
