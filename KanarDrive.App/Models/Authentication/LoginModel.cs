using System.ComponentModel.DataAnnotations;

namespace KanarDrive.App.Models.Authentication
{
    public class LoginModel
    {
        [Required] public string Email { get; set; }

        [Required] public string Password { get; set; }

        public bool Remember { get; set; }
    }
}