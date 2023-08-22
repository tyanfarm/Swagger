using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MyProject1.Validators;
using System.ComponentModel.DataAnnotations;

namespace MyProject1.Models
{
    public class StudentDTO
    {
        // Model Validation
        [ValidateNever]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Student name is required !")]
        [StringLength(100)]
        public string StudentName { get; set; }
        
        [EmailAddress(ErrorMessage = "Please enter the valid email address !")]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        //[DateCheck]
        //public DateTime AdmissionDate { get; set; }
        //[Range(10, 20)]        
        //public int Age { get; set; }

        //public string Password { get; set; }


        //[Compare(nameof(Password))]
        //public string ConfirmPassword { get; set; }
    }
}
