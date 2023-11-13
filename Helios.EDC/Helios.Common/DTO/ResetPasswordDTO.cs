using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Helios.Common.DTO
{
    public class ResetPasswordDTO
    {
        [Required]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bu alan zorunludur.")]
        [StringLength(16, ErrorMessage = "Şifre minumum 6 karakter uzunluğunda olmalı", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Şifre veya onay şifresi eşleşmiyor.")]
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Code { get; set; }
    }
}
