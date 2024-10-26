using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryDto : FullAuditedEntityDto<Guid>
    {
        public decimal BaseAmount { get; set; }

        public decimal? Bonus { get; set; }

        public decimal? Deduction { get; set; }
      
        public DateTime EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public decimal? TotalAmount { get; set; }

        public Guid EmployeeId { get; set; }
    }
}
