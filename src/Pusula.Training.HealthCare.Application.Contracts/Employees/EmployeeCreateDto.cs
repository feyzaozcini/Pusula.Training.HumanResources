using Pusula.Training.HealthCare.Employees;
using System;
using System.ComponentModel.DataAnnotations;


namespace Pusula.Training.HealthCare.Employees
{
    public class EmployeeCreateDto
    {
        [Required]
        [StringLength(EmployeeConst.FirstNameMaxLength, MinimumLength = EmployeeConst.FirstNameMinLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(EmployeeConst.LastNameMaxLength, MinimumLength = EmployeeConst.LastNameMinLength)]
        public string LastName { get; set; }
        
        [Required]
        [StringLength(EmployeeConst.IdentityNumberMaxLength)]
        public string IdentityNumber { get; set; }

        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(EmployeeConst.EmailMaxLength)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [RegularExpression(@"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$")]
        [StringLength(EmployeeConst.MobilePhoneNumberMaxLength)]
        public string MobilePhoneNumber { get; set; } = null!;

        [Required]
        public EnumGender Gender { get; set; }

        [Required]
        public Guid DepartmentId { get; set; }

    }
}
