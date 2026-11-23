
using HelpDesk.BLL.DTOs.Categories;
using HelpDesk.BLL.Entities;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.BLL.Interfaces.Services;

namespace HelpDesk.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync(
            bool activeOnly = true,
            CancellationToken cancellationToken = default)
        {
            var categories = await _categoryRepository.GetAllAsync(
                activeOnly,
                cancellationToken);

            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt
            }).ToList();
        }

        public async Task<CategoryDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid category ID.");
            }

            var category = await _categoryRepository.GetByIdAsync(
                id,
                cancellationToken);

            if (category is null)
            {
                return null;
            }

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                IsActive = category.IsActive,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<CategoryDto> CreateAsync(
            CreateCategoryDto category,
            string adminId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(category);

            if (string.IsNullOrWhiteSpace(adminId))
            {
                throw new UnauthorizedAccessException(
                    "Admin authentication is required.");
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException(
                    "Category name is required.");
            }

            var name = category.Name.Trim();

            if (name.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries).Length > 20)
            {
                throw new ArgumentException(
                    "Category name cannot exceed 20 words.");
            }

            var description = category.Description?.Trim();

            if (description?.Length > 500)
            {
                throw new ArgumentException(
                    "Category description cannot exceed 500 characters.");
            }

            if (await _categoryRepository.ExistsByNameAsync(
                name,
                cancellationToken: cancellationToken))
            {
                throw new InvalidOperationException(
                    "A category with this name already exists.");
            }

            var newCategory = new Category
            {
                Name = name,
                Description = description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _categoryRepository.AddAsync(
                newCategory,
                adminId,
                cancellationToken);

            return new CategoryDto
            {
                Id = newCategory.Id,
                Name = newCategory.Name,
                Description = newCategory.Description,
                IsActive = newCategory.IsActive,
                CreatedAt = newCategory.CreatedAt
            };
        }

        public async Task<bool> UpdateAsync(
            int id,
            UpdateCategoryDto category,
            string adminId,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(category);

            if (id <= 0)
            {
                throw new ArgumentException(
                    "Invalid category ID.");
            }

            if (string.IsNullOrWhiteSpace(adminId))
            {
                throw new UnauthorizedAccessException(
                    "Admin authentication is required.");
            }

            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException(
                    "Category name is required.");
            }

            var name = category.Name.Trim();

            if (name.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries).Length > 20)
            {
                throw new ArgumentException(
                    "Category name cannot exceed 20 words.");
            }

            var description = category.Description?.Trim();

            if (description?.Length > 500)
            {
                throw new ArgumentException(
                    "Category description cannot exceed 500 characters.");
            }

            var existingCategory =
                await _categoryRepository.GetByIdAsync(
                    id,
                    cancellationToken);

            if (existingCategory is null)
            {
                return false;
            }

            if (await _categoryRepository.ExistsByNameAsync(
                name,
                id,
                cancellationToken))
            {
                throw new InvalidOperationException(
                    "A category with this name already exists.");
            }

            existingCategory.Name = name;
            existingCategory.Description = description;
            existingCategory.IsActive = category.IsActive;

            await _categoryRepository.UpdateAsync(
                existingCategory,
                adminId,
                cancellationToken);

            return true;
        }
    }
}
