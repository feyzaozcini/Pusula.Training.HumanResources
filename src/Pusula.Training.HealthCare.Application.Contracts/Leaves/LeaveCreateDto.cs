using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Leaves
{
    public class LeaveCreateDto
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public LeaveType LeaveType { get; set; }

        public string Description { get; set; } = null!;

        public Guid EmployeeId { get; set; }
    }
}
