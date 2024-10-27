using System;
using Volo.Abp.Application.Dtos;

namespace Pusula.Training.HealthCare.Leaves
{
    public class GetLeavesInput : PagedAndSortedResultRequestDto
    {
        public string? FilterText { get; set; } = null!;

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public LeaveType? LeaveType { get; set; }

        public string? Description { get; set; } = null!;

        public Guid? EmployeeId { get; set; }

        public GetLeavesInput() { }
    }
}
