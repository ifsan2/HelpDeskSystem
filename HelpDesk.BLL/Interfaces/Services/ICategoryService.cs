using HelpDesk.BLL.DTOs.Categories;

namespace HelpDesk.BLL.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync(
            bool activeOnly = true,
            CancellationToken cancellationToken = default);

        Task<CategoryDto?> GetByIdAsync(
            int id,
            CancellationToken cancellationToken = default);

        Task<CategoryDto> CreateAsync(
            CreateCategoryDto category,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateAsync(
            int id,
            UpdateCategoryDto category,
            CancellationToken cancellationToken = default);
    }
}