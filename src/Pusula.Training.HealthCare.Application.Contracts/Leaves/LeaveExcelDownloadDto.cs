using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Leaves
{
    public class LeaveExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;

        public string? FilterText { get; set; } = null!;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public LeaveType? LeaveType { get; set; }

        public string? Description { get; set; } = null!;

        public Guid? EmployeeId { get; set; }

        public LeaveExcelDownloadDto()
        {
           
        }
    }
}
