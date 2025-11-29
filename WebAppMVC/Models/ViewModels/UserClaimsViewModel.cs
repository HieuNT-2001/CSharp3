namespace WebAppMVC.Models.ViewModels
{
    public class UserClaimsViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public List<UserClaim> Claims { get; set; } = new List<UserClaim>();
    }
}
