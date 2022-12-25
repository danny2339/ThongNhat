using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ThongNhatFinal.ModelViews
{
    public class RegisterVM
    {
        [Key]
        public int CustomerId { get; set; }
        [Display(Name = "FullName")]
        [Required(ErrorMessage = "Please Enter your fullname")]
        public string FullName { get; set; }
        [Required(ErrorMessage = "Please enter your Email")]
        [MaxLength(150)]
        [DataType(DataType.EmailAddress)]
        [Remote(action: "ValidateEmail", controller: "Accounts")]
        public string Email { get; set; }
        [MaxLength(11)]
        [Required(ErrorMessage = "Please enter your Phone Number")]
        [Display(Name = "Điện thoại")]
        [DataType(DataType.PhoneNumber)]
        [Remote(action: "ValidatePhone", controller: "Accounts")]
        public string Phone { get; set; }
        [Display(Name = "Password")]
        [Required(ErrorMessage = "Please enter your Password")]
        [MinLength(5, ErrorMessage = "You need to set a password with at least 5 characters")]
        public string Password { get; set; }
        [MinLength(5, ErrorMessage = "You need to set a password with at least 5 characters")]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Please enter the same password")]
        public string ConfirmPassword { get; set; }
    }
}
