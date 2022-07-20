using System.ComponentModel.DataAnnotations;

namespace PXLPRO2022Shoppers04.ViewModels
{
    public class RegisterViewModel
    {
        [Required] public string Firstname { get; set; }
        [Required] public string Lastname { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required] public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}