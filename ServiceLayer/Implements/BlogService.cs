using DataLayer.Entities;
using ServiceLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Implements
{
    public class BlogService : IBlogService
    {
        public Task<List<Blog>>? GetAllBlogsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Blog?> GetBlogByIdAsync(Guid blogId)
        {
            throw new NotImplementedException();
        }
    }
}
