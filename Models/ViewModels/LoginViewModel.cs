using System.ComponentModel.DataAnnotations;

namespace Veritabanı_proje.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email alanı zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre alanı zorunludur")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
} 