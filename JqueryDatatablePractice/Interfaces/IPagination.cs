using LINQExtensions.Models.ViewModels.JQueryDatatables;

namespace LINQExtensions.Interfaces
{
    public interface IPagination
    {
        IQueryable<T> GetPaginatedData<T>(IQueryable<T> query, DtRequestModel dt);
    }
}
