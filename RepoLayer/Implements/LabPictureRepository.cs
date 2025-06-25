using DataLayer.DbContext;
using DataLayer.Entities;
using RepoLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepoLayer.Implements
{
    public class LabPictureRepository : ILabPictureRepository
    {
        private readonly SWPSU25Context _context;

        public LabPictureRepository(SWPSU25Context context)
        {
            _context = context;
        }
        public async Task<LabPicture?> AddLabPictureAsync(LabPicture picture)
        {
            var entityEntry = await _context.LabPictures.AddAsync(picture);
            await _context.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}
