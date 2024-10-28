using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Salaries
{
    public class GetSalaryInput : PagedAndSortedResultRequestDto
    {
        //public string? FilterText { get; set; }
        public decimal? BaseAmount { get; set; }

        public decimal? Bonus { get; set; }

        public decimal? Deduction { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public Guid? EmployeeId { get; set; }

        public GetSalaryInput()
        {
        }
    }
}
