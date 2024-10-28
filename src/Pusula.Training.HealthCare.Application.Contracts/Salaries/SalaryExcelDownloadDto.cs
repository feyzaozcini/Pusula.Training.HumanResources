using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryExcelDownloadDto
    {
        public string DownloadToken { get; set; } = null!;
        //public string? FilterText { get; set; }
        public decimal? BaseAmount { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deduction { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public Guid EmployeeId { get; set; }

        public SalaryExcelDownloadDto() { }
    }
}
