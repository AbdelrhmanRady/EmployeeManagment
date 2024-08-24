using System.ComponentModel.DataAnnotations;
using WebApplication1.Models;

namespace WebApplication1.ViewModels
{
    public class EmployeeCreateViewModel
    {

        [Required]
        [MaxLength(20, ErrorMessage = "that's a long ass name")]
        public string? Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$", ErrorMessage = "That's not an email mate")]
        [Display(Name = "Your email bro")]
        public string? Email { get; set; }

        [Required]
        public Dept? Department { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
