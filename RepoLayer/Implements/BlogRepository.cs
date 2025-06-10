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
    public class BlogRepository : IBlogRepository
    {
        private readonly SWPSU25Context _context;

        public BlogRepository(SWPSU25Context context)
        {
            _context = context;
        }

        public async Task<List<Blog?>> GetAllBlogsAsync()
        {
            return await _context.Blogs.Include(b => b.AuthorID).ToListAsync();
        }

        public async Task<Blog?> GetBlogByIdAsync(Guid blogId)
        {
            return await _context.Blogs.Include(b => b.AuthorID)
                                       .FirstOrDefaultAsync(b => b.Id == blogId);
        }
    }
}
