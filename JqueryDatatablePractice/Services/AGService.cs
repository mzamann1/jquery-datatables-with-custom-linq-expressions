using LINQExtensions.Common;
using LINQExtensions.Interfaces;
using LINQExtensions.Models;

namespace LINQExtensions.Services
{
    public class AGService : IAGGridService
    {
        public List<User> GetUsers()
        {
            return CommonData.GetUsersList();
        }
    }
}
