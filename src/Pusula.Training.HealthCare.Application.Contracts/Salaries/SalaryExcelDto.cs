using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryExcelDto
    {
        public string DownloadToken { get; set; } = null!;

        public decimal BaseAmount { get; set; }

        public decimal Bonus { get; set; }

        public decimal Deduction { get; set; }

        public DateTime EffectiveFrom { get; set; } 

        public DateTime EffectiveTo { get; set; }

        public decimal TotalAmount { get; set; }

        public String Employee { get; set; }
    }
}
