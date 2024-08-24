using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [NotMapped]
        public string EncryptedId { get; set; }
        [Required]
        [MaxLength(20,ErrorMessage ="that's a long ass name")]
        public string? Name { get; set; }
        [Required]
        [RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$",ErrorMessage ="That's not an email mate")]
        [Display(Name ="Your email bro")]
        public string? Email { get; set; }

        [Required]
        public Dept? Department { get; set; }
        public string PhotoPath { get; set; }

    }
}
