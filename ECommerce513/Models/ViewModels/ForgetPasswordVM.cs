using System.ComponentModel.DataAnnotations;

namespace ECommerce513.Models.ViewModels
{
    public class ForgetPasswordVM
    {
        [Required]
        public string EmailOrUserName { get; set; } = null!;
    }
}
