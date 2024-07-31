using System.ComponentModel.DataAnnotations;


namespace Service.DTOs.Ui.Contacts
{
    public class ContactCreateDto
    {
        [Required]
        public string Name { get; set; }
        [Required, MinLength(1), DataType(DataType.EmailAddress), EmailAddress, MaxLength(50), Display(Name = "Email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = "Please enter a valid e-mail adress")]
        public string Email { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
