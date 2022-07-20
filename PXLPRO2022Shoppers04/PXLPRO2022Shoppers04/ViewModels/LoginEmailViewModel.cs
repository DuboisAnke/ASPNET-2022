using System.ComponentModel.DataAnnotations;

namespace PXLPRO2022Shoppers04.ViewModels
{
    public class LoginEmailViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}