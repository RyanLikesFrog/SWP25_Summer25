using DataLayer.DbContext;
using DataLayer.Entities;
using Microsoft.EntityFrameworkCore;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly SWPSU25Context _context;
        public UserRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByEmailAsync(string email) // <-- Triển khai
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                 .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
