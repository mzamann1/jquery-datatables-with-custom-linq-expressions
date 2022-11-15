using UserManagement.Core.Entities;
using UserManagement.Core.Interfaces;

namespace UserManagement.Infrastructure.Data;

internal class UserRepository : IUserRepository<User>
{
    public List<User> GetUsersList()
    {
        List<User> userList = new List<User>();

        for (int i = 0; i < 100; i++)
        {
            userList.Add(new User()
            {
                Id = Guid.NewGuid(),
                Name = $"Record {i + 1}",
                Username = $"user {i + 1}",
                Country = i % 2 == 0 ? "PK" : "US",
                Email = $"{i + 1}@example.com",
                Status = i % 2 == 0 ? "Active" : "In-Active",
                Balance = 100 + i

            });
        }
        return userList;
    }
}

