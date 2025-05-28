using DataLayer.Entities;
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
        public Task<List<Blog>> GetAllBlogsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Blog?> GetBlogByIdAsync(Guid blogId)
        {
            throw new NotImplementedException();
        }
    }
}
