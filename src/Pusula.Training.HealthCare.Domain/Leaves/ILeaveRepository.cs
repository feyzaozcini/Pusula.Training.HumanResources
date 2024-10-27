using Pusula.Training.HealthCare.Employees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Leaves
{
    public interface ILeaveRepository : IRepository<Leave, Guid>
    {
        Task DeleteAllAsync(
            string? filterText = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            LeaveType? leaveType= null,
            string? description= null,
            Guid? employeeId = null,
            CancellationToken cancellationToken = default );

        Task<LeaveWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<LeaveWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            LeaveType? leaveType = null,
            string? description = null,
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);


        Task<List<Leave>> GetListAsync(
            string? filterText = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            LeaveType? leaveType = null,
            string? description = null,
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            string? filterText = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            LeaveType? leaveType = null,
            string? description = null,
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
    }
}

