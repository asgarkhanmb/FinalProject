using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Ui.Contacts
{
    public class ContactCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
