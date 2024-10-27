using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Leaves
{
    public class EfCoreLeaveRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, Leave, Guid>(dbContextProvider), ILeaveRepository
    {
        public virtual async Task DeleteAllAsync(string? filterText = null, DateTime? startDate = null, DateTime? endDate = null, LeaveType? leaveType = null, string? description = null, Guid? employeeId = null, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, startDate, endDate, leaveType, description, employeeId);

            var ids = query.Select(x => x.Leave.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(string? filterText = null, DateTime? startDate = null, DateTime? endDate = null, LeaveType? leaveType = null, string? description = null, Guid? employeeId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, startDate,endDate,leaveType,description,employeeId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<Leave>> GetListAsync(string? filterText = null, DateTime? startDate = null, DateTime? endDate = null, LeaveType? leaveType = null, string? description = null, Guid? employeeId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, startDate, endDate, leaveType, description, employeeId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? LeaveConst.GetDefaultSorting(false) : sorting);
            return await query.Page(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<LeaveWithNavigationProperties>> GetListWithNavigationPropertiesAsync(string? filterText = null, DateTime? startDate = null, DateTime? endDate = null, LeaveType? leaveType = null, string? description = null, Guid? employeeId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, startDate,endDate,leaveType,description,employeeId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? EmployeeConst.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<LeaveWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(leave => new LeaveWithNavigationProperties
                {
                    Leave = leave,
                    Employee = dbContext.Set<Employee>().FirstOrDefault(c => c.Id == leave.EmployeeId)!,
                    
                })
                .FirstOrDefault()!;
        }


        #region ApplyFilter and Queryable
        protected virtual IQueryable<Leave> ApplyFilter(
            IQueryable<Leave> query,
            string? filterText = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            LeaveType? leaveType = null,
            string? description = null,
            Guid? employeeId = null) =>
                query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Description!.Contains(filterText!) )
                    .WhereIf(startDate.HasValue, e => e.StartDate == startDate!.Value)
                    .WhereIf(endDate.HasValue, e => e.EndDate == endDate!.Value)
                    .WhereIf(leaveType.HasValue, e => e.LeaveType == leaveType)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Description!.Contains(description!))
                    .WhereIf(employeeId.HasValue, e => e.EmployeeId == employeeId);
                    

        protected virtual async Task<IQueryable<LeaveWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
            from leave in (await GetDbSetAsync())
            join employee in (await GetDbContextAsync()).Set<Employee>() on leave.EmployeeId equals employee.Id into employees
            from employee in employees.DefaultIfEmpty()
            select new LeaveWithNavigationProperties
            {
                Leave = leave,
                Employee = employee
            };
            


        protected virtual IQueryable<LeaveWithNavigationProperties> ApplyFilter(
            IQueryable<LeaveWithNavigationProperties> query,
            string? filterText = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            LeaveType? leaveType = null,
            string? description = null,
            Guid? employeeId = null) =>
                query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Leave.Description!.Contains(filterText!))
                    .WhereIf(startDate.HasValue, e => e.Leave.StartDate == startDate!.Value)
                    .WhereIf(endDate.HasValue, e => e.Leave.EndDate == endDate!.Value)
                    .WhereIf(leaveType.HasValue, e => e.Leave.LeaveType == leaveType)
                    .WhereIf(!string.IsNullOrWhiteSpace(description), e => e.Leave.Description!.Contains(description!))
                    .WhereIf(employeeId.HasValue, e => e.Leave.EmployeeId == employeeId);
        #endregion
    }
}
