using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Admin.Subscribes
{
    public class SubscribeCreateDto
    {
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
    }
}
