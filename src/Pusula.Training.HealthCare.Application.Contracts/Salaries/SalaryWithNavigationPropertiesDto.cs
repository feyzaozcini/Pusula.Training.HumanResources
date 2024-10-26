using Pusula.Training.HealthCare.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryWithNavigationPropertiesDto
    {
        public SalaryDto Salary { get; set; } = null!;
        public EmployeeDto Employee { get; set; } = null!;
    }
}
