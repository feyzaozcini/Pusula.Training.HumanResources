using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;
namespace Pusula.Training.HealthCare.Leaves
{
    public class LeaveDto : FullAuditedEntityDto<Guid>
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
