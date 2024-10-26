
using System;

namespace Pusula.Training.HealthCare.Employees
{
    public class EmployeeExcelDto
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string IdentityNumber { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        public string Email { get; set; } = null!;

        public string MobilePhoneNumber { get; set; } = null!;

        public EnumGender Gender { get; set; }

        public String Department { get; set; }
    }
}
