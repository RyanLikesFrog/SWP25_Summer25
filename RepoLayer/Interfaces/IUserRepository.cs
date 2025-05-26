using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetUserByUsernameAsync(string username);
        public Task<User?> GetUserByEmailAsync(string email);
        public Task AddUserAsync(User user);
    }
}
