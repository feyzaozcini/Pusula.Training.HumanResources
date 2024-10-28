using Pusula.Training.HealthCare.Employees;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryWithNavigationPropertiesDto
    {
        public SalaryDto Salary { get; set; } = null!;
        public EmployeeDto Employee { get; set; } = null!;
    }
}
