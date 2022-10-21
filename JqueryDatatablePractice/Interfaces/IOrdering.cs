using LINQExtensions.Models.ViewModels.JQueryDatatables;

namespace LINQExtensions.Interfaces
{
    public interface IOrdering
    {
        IQueryable<T> GetOrderedData<T>(IQueryable<T> query, DtRequestModel dt);
    }
}
