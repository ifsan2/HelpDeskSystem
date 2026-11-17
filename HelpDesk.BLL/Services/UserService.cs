using HelpDesk.BLL.DTOs.Users;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.BLL.Interfaces.Services;

namespace HelpDesk.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IReadOnlyList<UserListDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetAllAsync(
                cancellationToken);
        }

        public async Task<bool> UpdateActiveStatusAsync(
            string userId,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID is required.");
            }

            return await _userRepository.UpdateActiveStatusAsync(
                userId,
                isActive,
                cancellationToken);
        }

        public async Task<bool> UpdateRoleAsync(
            string userId,
            string role,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID is required.");
            }

            if (role != "Employee" && role != "Technician")
            {
                throw new ArgumentException("Invalid user role.");
            }

            return await _userRepository.UpdateRoleAsync(
                userId,
                role,
                cancellationToken);
        }
    }
}