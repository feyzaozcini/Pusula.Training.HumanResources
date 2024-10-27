using Pusula.Training.HealthCare.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Leaves
{
    public class LeaveWithNavigationPropertiesDto
    {
        public LeaveDto Leave { get; set; } = null!;
        public EmployeeDto Employee { get; set; } = null!;
    }
}
