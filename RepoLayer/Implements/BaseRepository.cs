using DataLayer.DbContext;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class BaseRepository : IRepository
    {
        private readonly SWPSU25Context _context;
        public BaseRepository(SWPSU25Context context)
        {
            _context = context;
        }
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
