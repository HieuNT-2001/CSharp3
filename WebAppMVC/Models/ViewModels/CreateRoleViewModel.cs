using System.ComponentModel.DataAnnotations;

namespace WebAppMVC.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public required string RoleName { get; set; }
    }
}
