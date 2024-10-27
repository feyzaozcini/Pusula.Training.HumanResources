using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace Pusula.Training.HealthCare.Leaves
{
    public class Leave : FullAuditedAggregateRoot<Guid>
    {
        [NotNull]
        public virtual DateTime StartDate { get; set; }
        [NotNull]
        public virtual DateTime EndDate { get; set; }
        [NotNull]
        public virtual LeaveType LeaveType { get; set; }
        
        public virtual string Description { get; set; }

        public virtual Guid EmployeeId { get; set; }

        protected Leave()
        {
            StartDate = DateTime.Now;
            EndDate = DateTime.Now;
            LeaveType = LeaveType;
        }

        public Leave(Guid id, Guid employeeId, DateTime startDate, DateTime endDate, LeaveType leaveType, string description)
        {
            Check.NotNull(employeeId, nameof(employeeId));
            Check.NotNull(startDate, nameof(startDate));
            Check.NotNull(endDate, nameof(endDate));
            Check.Range((int)leaveType, nameof(leaveType), 1, 4);
            Check.NotNullOrWhiteSpace(description, nameof(description));

            Id = id;
            EmployeeId = employeeId;
            StartDate = startDate;
            EndDate = endDate;
            LeaveType = leaveType;
            Description = description;

        }

    }
}
