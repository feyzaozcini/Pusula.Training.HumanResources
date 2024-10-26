using Pusula.Training.HealthCare.Departments;

namespace Pusula.Training.HealthCare.Employees
{
    public class EmployeeWithNavigationPropertiesDto
    {
        public EmployeeDto Employee { get; set; } = null!;
        public DepartmentDto Department { get; set; } = null!;
    }
}
