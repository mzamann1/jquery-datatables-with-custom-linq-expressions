using JqueryDatatablePractice.Models.ViewModels;

namespace JqueryDatatablePractice.Interfaces
{
    public interface IFiltering
    {
        public IQueryable<T> GetFilteredData<T>(IQueryable<T> query, DtRequestModel dt, string value);
    }
}
