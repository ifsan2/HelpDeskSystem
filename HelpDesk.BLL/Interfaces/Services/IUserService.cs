using HelpDesk.BLL.DTOs.Users;

namespace HelpDesk.BLL.Interfaces.Services
{
    public interface IUserService
    {
        Task<IReadOnlyList<UserListDto>> GetAllAsync(
            CancellationToken cancellationToken = default);

        Task<bool> UpdateActiveStatusAsync(
            string userId,
            bool isActive,
            CancellationToken cancellationToken = default);

        Task<bool> UpdateRoleAsync(
            string userId,
            string role,
            CancellationToken cancellationToken = default);
    }
}