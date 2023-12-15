using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Helios.Authentication.Models
{
    public class ProfileViewModel
    {
        //[Required(ErrorMessage = "Bu alan zorunludur.")]
        [StringLength(16, ErrorMessage = "Şifre minumum 6 karakter uzunluğunda olmalı", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [Compare("ConfirmPassword", ErrorMessage = "Şifre veya onay şifresi eşleşmiyor." )]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        //[Required]
        public string UserName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        //[Required]
        public string ProfilPhotoPath { get; set; }
        public Int64 TimeZoneId { get; set; }
        public string TimeZone { get; set; }
        public int TimeZoneOffset { get; set; }
    }
}
