using System.Security.Claims;

namespace WebAppMVC.Models.ViewModels
{
    public class ClaimsStore
    {
        public static List<Claim> GetAllClaims()
        {
            return new List<Claim>
            {
                new Claim("Read Role", "Read Role"),
                new Claim("Create Role", "Create Role"),
                new Claim("Edit Role", "Edit Role"),
                new Claim("Delete Role", "Delete Role")
            };
        }
    }
}
