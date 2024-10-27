using System;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace Pusula.Training.HealthCare.Leaves
{
    public class LeaveManager(ILevaeRepository leaveRepository) : DomainService
    {
        public virtual async Task<Leave> CreateAsync(Guid employeeId, DateTime startDate, DateTime endDate, LeaveType leaveType, string description)
        {
            Check.NotNull(employeeId, nameof(employeeId));
            Check.NotNull(startDate, nameof(startDate));
            Check.NotNull(endDate, nameof(endDate));
            Check.Range((int)leaveType, nameof(leaveType), 1, 4);
            Check.NotNullOrWhiteSpace(description, nameof(description));

            var leave = new Leave(
                GuidGenerator.Create(), employeeId, startDate, endDate, leaveType, description
            );

            return await leaveRepository.InsertAsync(leave);
        }

        public virtual async Task<Leave> UpdateAsync(
            Guid id, Guid employeeId, DateTime startDate, DateTime endDate, LeaveType leaveType, string description)

        {
            Check.NotNull(employeeId, nameof(employeeId));
            Check.NotNull(startDate, nameof(startDate));
            Check.NotNull(endDate, nameof(endDate));
            Check.Range((int)leaveType, nameof(leaveType), 1, 4);
            Check.NotNullOrWhiteSpace(description, nameof(description));

            var leave = await leaveRepository.GetAsync(id);

            leave.EmployeeId = employeeId;
            leave.StartDate = startDate;
            leave.EndDate = endDate;
            leave.LeaveType = leaveType;
            leave.Description = description;

            return await leaveRepository.UpdateAsync(leave);
        }

    }
}
