using Pusula.Training.HealthCare.Employees;
using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Employees
{
    public class EmployeeDto : FullAuditedEntityDto<Guid>
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string IdentityNumber { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string Email { get; set; } = null!;

        public string MobilePhoneNumber { get; set; } = null!;

        public EnumGender? Gender { get; set; }

        public Guid DepartmentId { get; set; }


        
    }
}
