using System;
using System.ComponentModel.DataAnnotations;

namespace LoginProject.Models
{
	public class LoginViewModel
	{
        [Required]
        [Display(Name = "İsim")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Soyisim")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Kullanıcı Adı")]
        public string  Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }

        [Display(Name = "IP Adresi")]
        public string IPAddress { get; set; }

        [Display(Name = "Giriş Tarihi")]
        public DateTime LoginTime { get; set; }

        [Display(Name = "Duration")]
        public DateTime Duration { get; set; }

    }
}
