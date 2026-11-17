using Microsoft.AspNetCore.Identity;

namespace HelpDesk.DAL.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string? Department { get; set; }

        public bool IsActive { get; set; } = true;
    }
}