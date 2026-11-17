using HelpDesk.BLL.DTOs.Users;
using HelpDesk.BLL.Interfaces.Repositories;
using HelpDesk.DAL.Data;
using HelpDesk.DAL.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.DAL.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IReadOnlyList<UserListDto>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(u => u.FirstName)
                .ToListAsync(cancellationToken);

            var userRoles = await _context.UserRoles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            var roles = await _context.Roles
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return users.Select(user =>
            {
                var roleId = userRoles
                    .FirstOrDefault(ur => ur.UserId == user.Id)?.RoleId;

                var roleName = roles
                    .FirstOrDefault(r => r.Id == roleId)?.Name;

                return new UserListDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    Department = user.Department,
                    IsActive = user.IsActive,
                    Role = roleName ?? "No Role"
                };
            }).ToList();
        }

        public async Task<bool> UpdateActiveStatusAsync(
            string userId,
            bool isActive,
            CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == userId,
                    cancellationToken);

            if (user == null)
            {
                return false;
            }

            user.IsActive = isActive;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> UpdateRoleAsync(
            string userId,
            string role,
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            if (role != "Employee" && role != "Technician")
            {
                return false;
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Protect administrator accounts.
            if (currentRoles.Contains("Admin"))
            {
                return false;
            }

            if (currentRoles.Contains(role))
            {
                return true;
            }

            // Assign the new role first.
            var addResult = await _userManager.AddToRoleAsync(user, role);

            if (!addResult.Succeeded)
            {
                return false;
            }

            // Remove the previous non-admin roles.
            var rolesToRemove = currentRoles
                .Where(r => r != "Admin")
                .ToArray();

            if (rolesToRemove.Length > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(
                    user, rolesToRemove);

                if (!removeResult.Succeeded)
                {
                    await _userManager.RemoveFromRoleAsync(user, role);
                    return false;
                }
            }

            return true;
        }
    }
}