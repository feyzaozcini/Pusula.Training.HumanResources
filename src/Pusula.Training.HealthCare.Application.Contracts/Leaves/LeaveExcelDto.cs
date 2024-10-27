using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Leaves
{
    public class LeaveExcelDto
    {
        
        public DateTime StartDate { get; set; }
        
        public DateTime EndDate { get; set; }
       
        public LeaveType LeaveType { get; set; }

        public string Description { get; set; } = null!;

        
        public string Employee { get; set; } = null!;
    }
}
