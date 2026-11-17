using HelpDesk.DAL.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HelpDesk.Web.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public string Email { get; set; } = string.Empty;

        [BindProperty]
        public string Password { get; set; } = string.Empty;

        [BindProperty]
        public bool RememberMe { get; set; }

        public string? ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Email.Trim());

            if (user == null || !user.IsActive)
            {
                ErrorMessage = "Invalid email or password.";
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                Password,
                isPersistent: RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);

                if (roles.Contains("Admin"))
                {
                    return Redirect("/admin/dashboard");
                }

                if (roles.Contains("Technician"))
                {
                    return Redirect("/technician/dashboard");
                }

                if (roles.Contains("Employee"))
                {
                    return Redirect("/employee/my-tickets");
                }

                await _signInManager.SignOutAsync();

                ErrorMessage = "Your account does not have an assigned role.";
                return Page();
            }

            ErrorMessage = result.IsLockedOut
                ? "Your account is temporarily locked. Please try again later."
                : "Invalid email or password.";

            return Page();
        }
    }
}