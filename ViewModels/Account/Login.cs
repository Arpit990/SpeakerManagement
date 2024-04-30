using System.ComponentModel.DataAnnotations;

namespace SpeakerManagement.ViewModels.Account
{
    public class Login
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Please Enter Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Please Enter Password")]
        public string Password { get; set; }
    }
}
