using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dto;
using Application.Interfaces;
using Application.Mappers;
using Domain.Models.Entities;

namespace Application.UseCases
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _repository;

        public BlogService(IBlogRepository repository)
        {
            _repository = repository;
        }

        public async Task CreateArticleAsync(PostCreateDto articleDto)
        {
            // MAPPATURA: DTO -> Domain Entity
            var entity = articleDto.ToCreateEntity();

            await _repository.SaveAsync(entity);
        }

        public async Task<PostReadDto?> GetArticleByIdAsync(string id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            return entity.ToReadDto();
        }

        public async Task<IEnumerable<PostReadDto>> GetAllArticlesAsync()
        {
            var entities = await _repository.GetAllAsync();

            // MAPPATURA: Collection Entity -> Collection DTO
            return entities.Select(e => e.ToReadDto());
        }

        public async Task UpdateArticleAsync(string id, PostCreateDto articleDto)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null)
                throw new InvalidOperationException($"Articolo {id} non trovato");

            entity.Title = articleDto.Title;
            entity.Content = articleDto.Content;
            entity.ReloadDate();

            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteArticleAsync(string id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IEnumerable<PostReadDto>> GetByTitleAsync(string title)
        {
            var titleArt = await _repository.GetByTitleAsync(title);
            return titleArt.Select(e => e.ToReadDto());
        }

        public async Task<IEnumerable<PostReadDto>> GetByContentAsync(string content)
        {
            var contentArt = await _repository.GetByContentAsync(content);
            return contentArt.Select(e => e.ToReadDto());
        }

        public async Task<IEnumerable<PostReadDto>> GetByDateAsync(DateTime date)
        {
            var dateArt = await _repository.GetByDateAsync(date);
            return dateArt.Select(e => e.ToReadDto());
        }

        public async Task<int> CountByDateAsync(DateTime date)
        {
            var count = await _repository.CountByDateAsync(date);
            return count;
        }

        public async Task<IEnumerable<PostReadDto>> GetInPeriodAsync(DateTime start, DateTime end)
        {
            var periodArt = await _repository.GetInPeriodAsync(start, end);
            return periodArt.Select(e => e.ToReadDto());
        }
    }
}

