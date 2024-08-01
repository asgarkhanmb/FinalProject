using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Ui.Contacts
{
    public class ContactCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
