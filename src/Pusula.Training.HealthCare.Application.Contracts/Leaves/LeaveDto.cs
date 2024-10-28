using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
namespace Pusula.Training.HealthCare.Leaves
{
    public class LeaveDto : FullAuditedEntityDto<Guid>, IHasConcurrencyStamp
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public LeaveType LeaveType { get; set; }

        public string Description { get; set; } = null!;

        public Guid EmployeeId { get; set; }

        public string ConcurrencyStamp { get; set; } = null!;
    }
}
