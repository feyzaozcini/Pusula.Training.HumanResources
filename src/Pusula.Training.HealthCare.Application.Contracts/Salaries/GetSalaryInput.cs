using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Salaries
{
    public class GetSalaryInput : PagedAndSortedResultRequestDto
    {
        public decimal? BaseAmountMin { get; set; }
        public decimal? BaseAmountMax { get; set; }
        public decimal? BonusMin { get; set; }
        public decimal? BonusMax { get; set; }
        public decimal? DeductionMin { get; set; }
        public decimal? DeductionMax  { get; set;}
        public DateTime? EffectiveFromMin { get; set; }
        public DateTime? EffectiveFromMax { get; set; }
        public DateTime? EffectiveToMin { get; set; }
        public DateTime? EffectiveToMax { get; set; }
        public decimal? TotalAmountMin { get; set; }
        public decimal? TotalAmountMax { get; set; }
        public Guid? EmployeeId { get; set; }

        public GetSalaryInput()
        {
        }
    }
}
