using System.ComponentModel.DataAnnotations;

namespace ThongNhatFinal.ModelViews
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name = "Current Password")]
        public string PasswordNow { get; set; }
        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Please enter your Password")]
        [MinLength(5, ErrorMessage = "You need to set a password with at least 5 characters")]
        public string Password { get; set; }
        [MinLength(5, ErrorMessage = "You need to set a password with at least 5 characters")]
        [Display(Name = "Confirm New Password")]
        [Compare("Password", ErrorMessage = "Please enter the same password")]
        public string ConfirmPassword { get; set; }
    }

}
