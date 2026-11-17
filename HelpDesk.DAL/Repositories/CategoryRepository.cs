using HelpDesk.BLL.Entities;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DAL.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Category>> GetAllAsync(
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<Category> query = _context.Categories.AsNoTracking();

            if (activeOnly)
            {
                query = query.Where(c => c.IsActive);
            }

            return await query
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<Category?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }

        public async Task<bool> ExistsByNameAsync(
            string name,
            int? excludeId = null,
            CancellationToken cancellationToken = default)
        {
            return await _context.Categories.AnyAsync(
                c => c.Name == name &&
                     (!excludeId.HasValue || c.Id != excludeId.Value),
                cancellationToken);
        }

        public async Task AddAsync(
            Category category,
            CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(category, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(
            Category category,
            CancellationToken cancellationToken = default)
        {
            // FindAsync first checks whether this category
            // is already being tracked by the DbContext.
            var existingCategory = await _context.Categories.FindAsync(
                new object?[] { category.Id },
                cancellationToken);

            if (existingCategory is null)
            {
                return;
            }

            // Update the existing tracked entity.
            existingCategory.Name = category.Name;
            existingCategory.Description = category.Description;
            existingCategory.IsActive = category.IsActive;

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}