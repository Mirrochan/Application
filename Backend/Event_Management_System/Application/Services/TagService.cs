using Application.Abstractions;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        public TagService(ITagRepository tagRepository)
        {
            _tagRepository = tagRepository;
        }
        public async Task<ICollection<TagModel>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllTagsAsync();
        }
    }
}
