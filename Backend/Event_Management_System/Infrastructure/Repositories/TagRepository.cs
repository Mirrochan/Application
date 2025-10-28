using Application.Abstractions;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly EventManagmentSystemDbContext _context;
        public TagRepository(EventManagmentSystemDbContext context)
        {
            _context = context;
        }
        public async Task<ICollection<TagModel>> GetAllTagsAsync()
        {
            return await _context.Tags.ToListAsync();
        }

        public Task<TagModel> GetTagByIdAsync(Guid tagId)
        {
            return _context.Tags.FirstOrDefaultAsync(t => t.Id == tagId);
        }

    }
}
