namespace WebAppMVC.Models
{
    public class EditRoleViewModel
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public List<RoleItem> Roles { get; set; } = [];
    }
}
