using LINQExtensions.Models.ViewModels.JQueryDatatables;

namespace LINQExtensions.Interfaces
{
    public interface IFiltering
    {
        public IQueryable<T> GetFilteredData<T>(IQueryable<T> query, JQueryDtRequest dt);
    }
}
