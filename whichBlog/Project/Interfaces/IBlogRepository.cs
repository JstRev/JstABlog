using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Entities;

namespace Application.Interfaces
{
    public interface IBlogRepository
    {
        Task SaveAsync(Post article);
        Task<Post?> GetByIdAsync(string id);
        Task<IEnumerable<Post>> GetAllAsync();
        Task UpdateAsync(Post article);
        Task DeleteAsync(string id);

        Task<IEnumerable<Post>> GetByTitleAsync(string title);

        Task<IEnumerable<Post>> GetByContentAsync(string content);

        Task<IEnumerable<Post>> GetByDateAsync(DateTime date);

        Task<int> CountByDateAsync(DateTime date);

        Task<IEnumerable<Post>> GetInPeriodAsync(DateTime start, DateTime end);

    }
}
