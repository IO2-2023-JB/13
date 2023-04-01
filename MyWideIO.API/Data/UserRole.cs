using Microsoft.AspNetCore.Identity;

namespace MyWideIO.API.Data
{
    public class UserRole : IdentityRole<Guid>
    {
        public UserRole() : base()
        {

        }
        public UserRole(string roleName) : base(roleName)
        {

        }
    }
}
