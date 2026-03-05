using Application.Dto;
using Domain.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    //descrive tutti i metodi che costituiranno il service, utile per la build 
    public interface IBlogService
    {
        public Task CreateArticleAsync(PostCreateDto articleDto);

        public Task<PostReadDto?> GetArticleByIdAsync(string id);

        public Task<IEnumerable<PostReadDto>> GetAllArticlesAsync();

        public Task UpdateArticleAsync(string id, PostCreateDto articleDto);

        public Task DeleteArticleAsync(string id);

        public Task<IEnumerable<PostReadDto>> GetByTitleAsync(string title);

        public Task<IEnumerable<PostReadDto>> GetByContentAsync(string content);

        public Task<IEnumerable<PostReadDto>> GetByDateAsync(DateTime date);

        public Task<int> CountByDateAsync(DateTime date);

        public Task<IEnumerable<PostReadDto>> GetInPeriodAsync(DateTime start, DateTime end);
    }
}
