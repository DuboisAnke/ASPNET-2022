using System.ComponentModel.DataAnnotations;

namespace PXLPRO2022Shoppers04.ViewModels
{
    public class LoginUsernameViewModel
    {
        [Required] public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}