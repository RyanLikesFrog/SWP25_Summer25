using DataLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Interfaces
{
    public interface IBlogRepository
    {
        public Task<Blog?> GetBlogByIdAsync(Guid blogId);
        public Task<List<Blog?>> GetAllBlogsAsync();

    }
}
