using UserManagement.Core.Models.RequestModels.JQuery;

namespace UserManagement.Core.Interfaces;
public interface IFiltering
{
    public IQueryable<T> GetFilteredData<T>(IQueryable<T> query, JQueryDtRequest dt);
}
