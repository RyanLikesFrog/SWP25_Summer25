using DataLayer.Entities;
using ServiceLayer.DTOs.User.Request;
using ServiceLayer.DTOs.User.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Interfaces
{
    public interface IUserService
    {
        public Task<CreateUserResponse> CreateUserAccountByAdminAsync(CreateAccountByAdminRequest request);
        public Task<User?> GetUserByUsernameAsync(string username);
        public Task<User?> GetUserByEmailAsync(string email);
    }
}
