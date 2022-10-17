using JqueryDatatablePractice.Models.ViewModels;

namespace JqueryDatatablePractice.Interfaces
{
    public interface IFiltering
    {
        public IQueryable<T> GetFilteredData<T>(IQueryable<T> query, DtRequest dt, string value);
    }
}
