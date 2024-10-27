
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Employees
{
    public class EmployeeUpdateDto : IHasConcurrencyStamp
    {
        [Required]
        public Guid Id { get; set; } = default!;

        [Required]
        [StringLength(EmployeeConst.FirstNameMaxLength, MinimumLength = EmployeeConst.FirstNameMinLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(EmployeeConst.LastNameMaxLength, MinimumLength = EmployeeConst.LastNameMinLength)]
        public string LastName { get; set; }

        [Required]
        [RegularExpression(@"^[1-9]{1}[0-9]{9}[02468]{1}$")]
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

        public string ConcurrencyStamp { get; set; } = null!;
    }
}
