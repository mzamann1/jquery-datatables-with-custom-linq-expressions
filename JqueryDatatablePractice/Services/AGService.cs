using JqueryDatatablePractice.Common;
using JqueryDatatablePractice.Constants;
using JqueryDatatablePractice.Interfaces;
using JqueryDatatablePractice.Models;
using JqueryDatatablePractice.Models.ViewModels;

namespace JqueryDatatablePractice.Services
{
    public class AGService : IAGGridService
    {
        public List<User> GetUsers()
        {
            return CommonData.GetUsersList();
        }
    }
}
