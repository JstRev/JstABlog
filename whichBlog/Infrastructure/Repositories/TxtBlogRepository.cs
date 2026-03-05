using Domain.Models.Entities;
using Infrastructure.Mapper;

namespace Infrastructure.Repositories
{
    public class TxtBlogRepository : BaseRepository
    {
        public TxtBlogRepository(string? filePath = null)
            : base(filePath, "articles.txt")
        {
        }

        public override async Task SaveAsync(Post article)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();
                var dto = article.ToPersistenceDto();
                articles[article.Id.ToString()] = dto;
                await SaveToFileAsync(articles);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override async Task<Post?> GetByIdAsync(string id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();

                if (!articles.TryGetValue(id, out var dto))
                    return null;

                return dto.ToEntity();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override async Task<IEnumerable<Post>> GetAllAsync()
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();

                return articles.Values
                    .Select(a => a.ToEntity())
                    .OrderByDescending(a => a.CreatedAt)
                    .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override async Task UpdateAsync(Post article)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();
                var idKey = article.Id.ToString();

                if (!articles.ContainsKey(idKey))
                    throw new InvalidOperationException($"Articolo con ID {article.Id} non trovato");

                articles[idKey] = article.ToPersistenceDto();

                await SaveToFileAsync(articles);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override async Task DeleteAsync(string id)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();

                if (!articles.ContainsKey(id))
                    throw new InvalidOperationException($"Articolo con ID {id} non trovato");

                articles.Remove(id);

                await SaveToFileAsync(articles);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override async Task<IEnumerable<Post>> GetByTitleAsync(string title)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();
                var posts = articles.Values.Select(a => a.ToEntity());
                return posts
                    .Where(p => !string.IsNullOrEmpty(p.Title) && p.Title.IndexOf(title ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override async Task<IEnumerable<Post>> GetByContentAsync(string content)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();
                var posts = articles.Values.Select(a => a.ToEntity());
                return posts
                    .Where(p => !string.IsNullOrEmpty(p.Content) && p.Content.IndexOf(content ?? string.Empty, StringComparison.OrdinalIgnoreCase) >= 0)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override async Task<IEnumerable<Post>> GetByDateAsync(DateTime date)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();
                var posts = articles.Values.Select(a => a.ToEntity());
                var targetDate = date.Date;
                return posts
                    .Where(p => p.CreatedAt.Date == targetDate)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override async Task<int> CountByDateAsync(DateTime date)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();
                var posts = articles.Values.Select(a => a.ToEntity());
                var targetDate = date.Date;
                return posts.Count(p => p.CreatedAt.Date == targetDate);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public override async Task<IEnumerable<Post>> GetInPeriodAsync(DateTime start, DateTime end)
        {
            await _semaphore.WaitAsync();
            try
            {
                var articles = await LoadFromFileAsync();
                var posts = articles.Values.Select(a => a.ToEntity());
                var from = start;
                var to = end;
                if (from > to)
                {
                    // normalizzo scambio se necessario
                    var tmp = from;
                    from = to;
                    to = tmp;
                }

                return posts
                    .Where(p => p.CreatedAt >= from && p.CreatedAt <= to)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}