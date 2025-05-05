using System.ComponentModel.DataAnnotations;

namespace ECommerce513.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string EmailOrUserName { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public bool RememberMe { get; set; }
    }
}
