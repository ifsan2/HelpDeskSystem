
using HelpDesk.BLL.Entities;

namespace HelpDesk.BLL.Interfaces.Repositories
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<Category>> GetAllAsync(
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<Category?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(
            string name,
            int? excludeId = null,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            Category category,
            string adminId,
            CancellationToken cancellationToken = default);

        Task UpdateAsync(
            Category category,
            string adminId,
            CancellationToken cancellationToken = default);
    }
}
