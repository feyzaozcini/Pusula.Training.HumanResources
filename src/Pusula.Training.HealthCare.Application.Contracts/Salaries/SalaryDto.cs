using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        [Required]
        public decimal BaseAmount { get; set; } 

        public decimal Bonus { get; set; }

        public decimal Deduction { get; set; }
        [Required]
        public DateTime EffectiveFrom { get; set; }

        public DateTime EffectiveTo { get; set; }
        [Required]
        public Guid EmployeeId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}
