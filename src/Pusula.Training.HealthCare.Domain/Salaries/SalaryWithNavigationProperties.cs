using Pusula.Training.HealthCare.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryWithNavigationProperties
    {
        public Salary Salary { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
    }
}
