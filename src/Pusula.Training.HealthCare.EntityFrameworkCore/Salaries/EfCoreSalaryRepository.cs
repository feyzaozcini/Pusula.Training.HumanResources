using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Salaries
{
    public class EfCoreSalaryRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, Salary, Guid>(dbContextProvider), ISalaryRepository
    {
        public virtual async Task<List<Salary>> GetListAsync(decimal? baseAmount = null, decimal? bonus = null, decimal? deduction = null, DateTime? effectiveFrom = null, DateTime? effectiveTo = null, Guid? employeeId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), baseAmount,bonus, deduction,effectiveFrom,effectiveTo ,employeeId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SalaryConst.GetDefaultSorting(false) : sorting);
            return await query.Page(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }
        public virtual async Task<SalaryWithNavigationProperties> GetWithNavigationPropertiesAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(salary => new SalaryWithNavigationProperties
                {
                    Salary = salary,
                    Employee = dbContext.Set<Employee>().FirstOrDefault(c => c.Id == salary.EmployeeId)!
                })
                .FirstOrDefault()!;
        }
        public virtual async Task<List<SalaryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(decimal? baseAmount = null, decimal? bonus = null, decimal? deduction = null, DateTime? effectiveFrom = null, DateTime? effectiveTo = null, Guid? employeeId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, baseAmount, bonus,deduction,effectiveFrom,effectiveTo,employeeId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SalaryConst.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }
        public virtual async Task<long> GetCountAsync(decimal? baseAmount = null, decimal? bonus = null, decimal? deduction = null, DateTime? effectiveFrom = null, DateTime? effectiveTo = null, Guid? employeeId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, baseAmount,bonus,deduction,effectiveFrom,effectiveTo, employeeId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }
        public virtual async Task DeleteAllAsync(decimal? baseAmount = null, decimal? bonus = null, decimal? deduction = null, DateTime? effectiveFrom = null, DateTime? effectiveTo = null, Guid? employeeId = null, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, baseAmount,bonus,deduction,effectiveFrom,effectiveTo, employeeId);

            var ids = query.Select(x => x.Salary.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        #region ApplyFilter and Queryable
        protected virtual IQueryable<Salary> ApplyFilter(
            IQueryable<Salary> query,
            decimal? baseAmount = null,
            decimal? bonus = null,
            decimal? deduction = null,
            DateTime? effectiveFrom = null,
            DateTime? effectiveTo = null,
            Guid? employeeId = null) =>
                query
                     .WhereIf(employeeId.HasValue, e => e.EmployeeId == employeeId);


        protected virtual async Task<IQueryable<SalaryWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
            from salary in (await GetDbSetAsync())
            join employee in (await GetDbContextAsync()).Set<Employee>() on salary.EmployeeId equals employee.Id into employees
            from employee in employees.DefaultIfEmpty()
            select new SalaryWithNavigationProperties
            {
                Salary = salary,
                Employee = employee
            };

        protected virtual IQueryable<SalaryWithNavigationProperties> ApplyFilter(
            IQueryable<SalaryWithNavigationProperties> query,
            decimal? baseAmount = null,
            decimal? bonus = null,
            decimal? deduction = null,
            DateTime? effectiveFrom = null,
            DateTime? effectiveTo = null,
            Guid? employeeId = null
            ) =>
                query
                    .WhereIf(employeeId.HasValue, e => e.Salary.EmployeeId == employeeId);

        
    }

    #endregion
}

