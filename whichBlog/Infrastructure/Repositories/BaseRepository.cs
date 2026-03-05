using Application.Interfaces;
using Domain.Models.Entities;
using Infrastructure.Dto;
using System.Text.Json;

namespace Infrastructure.Repositories
{
    public abstract class BaseRepository : IBlogRepository
    {
        protected string _filePath;
        protected readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        protected static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        protected BaseRepository(string? filePath, string defaultFileName)
        {
            _filePath = filePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "BlogProject",
                defaultFileName
            );

            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            if (!File.Exists(_filePath))
            {
                SaveToFileAsync(new Dictionary<string, PostPersistenceDto>()).GetAwaiter().GetResult();
            }
        }

        public abstract Task SaveAsync(Post article);
        public abstract Task<Post?> GetByIdAsync(string id);
        public abstract Task<IEnumerable<Post>> GetAllAsync();
        public abstract Task UpdateAsync(Post article);
        public abstract Task DeleteAsync(string id);
        public abstract Task<IEnumerable<Post>> GetByTitleAsync(string title);
        public abstract Task<IEnumerable<Post>> GetByContentAsync(string content);
        public abstract Task<IEnumerable<Post>> GetByDateAsync(DateTime date);
        public abstract Task<int> CountByDateAsync(DateTime date);
        public abstract Task<IEnumerable<Post>> GetInPeriodAsync(DateTime start, DateTime end);

        protected async Task<Dictionary<string, PostPersistenceDto>> LoadFromFileAsync()
        {
            if (!File.Exists(_filePath))
                return new Dictionary<string, PostPersistenceDto>();

            using FileStream openStream = File.OpenRead(_filePath);

            if (openStream.Length == 0)
                return new Dictionary<string, PostPersistenceDto>();

            return await JsonSerializer.DeserializeAsync<Dictionary<string, PostPersistenceDto>>(openStream, _jsonOptions)
                    ?? new Dictionary<string, PostPersistenceDto>();
        }

        protected async Task SaveToFileAsync(Dictionary<string, PostPersistenceDto> articles)
        {
            using FileStream createStream = File.Create(_filePath);
            await JsonSerializer.SerializeAsync(createStream, articles, _jsonOptions);
        }
    }
}