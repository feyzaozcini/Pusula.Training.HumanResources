using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Employees
{
    public class GetEmployeeInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? IdentityNumber { get; set; }

        public DateTime? BirthDateMin { get; set; }

        public DateTime? BirthDateMax { get; set; }

        public string? Email { get; set; }

        public string? MobilePhoneNumber { get; set; }

        public EnumGender? Gender { get; set; }

        public Guid? DepartmentId { get; set; }


        public GetEmployeeInput()
        {
        }
    }
}
