namespace WebAppMVC.Models.ViewModels
{
    public class UserClaim
    {
        public string ClaimType { get; set; } = string.Empty;
        public string ClaimValue { get; set; } = string.Empty;
        public bool IsSelected { get; set; }
    }
}
