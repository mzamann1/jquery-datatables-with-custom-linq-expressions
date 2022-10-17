namespace JqueryDatatablePractice.Interfaces
{
    public interface IPagination
    {
        IQueryable<T> GetPaginatedData<T>(IQueryable<T> query, int start, int end);
    }
}
