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
        public virtual async  Task DeleteAllAsync(decimal? baseAmountMin = null, decimal? baseAmountMax = null, decimal? bonusMin = null, decimal? bonusMax = null, decimal? deductionMin = null, decimal? deductionMax = null, DateTime? effectiveFromMin = null, DateTime? effectiveFromMax = null, DateTime? effectiveToMin = null, DateTime? effectiveToMax = null, decimal? totalAmountMin = null, decimal? totalAmountMax = null, Guid? employeeId = null, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, baseAmountMin, baseAmountMax, bonusMin, bonusMax, deductionMin, deductionMax, effectiveFromMin, effectiveFromMax, effectiveToMin, effectiveToMax, totalAmountMin, totalAmountMax, employeeId);

            var ids = query.Select(x => x.Salary.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));

        }

        public virtual async Task<long> GetCountAsync(decimal? baseAmountMin = null, decimal? baseAmountMax = null, decimal? bonusMin = null, decimal? bonusMax = null, decimal? deductionMin = null, decimal? deductionMax = null, DateTime? effectiveFromMin = null, DateTime? effectiveFromMax = null, DateTime? effectiveToMin = null, DateTime? effectiveToMax = null, decimal? totalAmountMin = null, decimal? totalAmountMax = null, Guid? employeeId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, baseAmountMin, baseAmountMax, bonusMin, bonusMax, deductionMin, deductionMax, effectiveFromMin, effectiveFromMax, effectiveToMin, effectiveToMax, totalAmountMin, totalAmountMax, employeeId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<Salary>> GetListAsync(decimal? baseAmountMin = null, decimal? baseAmountMax = null, decimal? bonusMin = null, decimal? bonusMax = null, decimal? deductionMin = null, decimal? deductionMax = null, DateTime? effectiveFromMin = null, DateTime? effectiveFromMax = null, DateTime? effectiveToMin = null, DateTime? effectiveToMax = null, decimal? totalAmountMin = null, decimal? totalAmountMax = null, Guid? employeeId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query= ApplyFilter(await GetQueryableAsync(), baseAmountMin, baseAmountMax, bonusMin, bonusMax, deductionMin, deductionMax, effectiveFromMin, effectiveFromMax, effectiveToMin, effectiveToMax, totalAmountMin, totalAmountMax, employeeId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SalaryConst.GetDefaultSorting(false) : sorting);
            return await query.Page(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<SalaryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(decimal? baseAmountMin = null, decimal? baseAmountMax = null, decimal? bonusMin = null, decimal? bonusMax = null, decimal? deductionMin = null, decimal? deductionMax = null, DateTime? effectiveFromMin = null, DateTime? effectiveFromMax = null, DateTime? effectiveToMin = null, DateTime? effectiveToMax = null, decimal? totalAmountMin = null, decimal? totalAmountMax = null, Guid? employeeId = null, string? sorting = null, int maxResultCount = int.MaxValue, int skipCount = 0, CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, baseAmountMin, baseAmountMax, bonusMin, bonusMax, deductionMin, deductionMax, effectiveFromMin, effectiveFromMax, effectiveToMin, effectiveToMax, totalAmountMin, totalAmountMax, employeeId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? SalaryConst.GetDefaultSorting(true) : sorting);
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

        #region ApplyFilter and Queryable
        protected virtual IQueryable<Salary> ApplyFilter(
            IQueryable<Salary> query,
            decimal? baseAmountMin = null,
            decimal? baseAmountMax = null,
            decimal? bonusMin = null,
            decimal? bonusMax = null,
            decimal? deductionMin = null,
            decimal? deductionMax = null,
            DateTime? effectiveFromMin = null,
            DateTime? effectiveFromMax = null,
            DateTime? effectiveToMin = null,
            DateTime? effectiveToMax = null,
            decimal? totalAmountMin = null,
            decimal? totalAmountMax = null,
            Guid? employeeId = null) =>
                query
                    .WhereIf(baseAmountMin.HasValue, e => e.BaseAmount >= baseAmountMin!.Value)
                    .WhereIf(baseAmountMax.HasValue, e => e.BaseAmount <= baseAmountMax!.Value)
                    .WhereIf(bonusMin.HasValue, e => e.Bonus >= bonusMin!.Value)
                    .WhereIf(bonusMax.HasValue, e => e.Bonus <= bonusMax!.Value)
                    .WhereIf(deductionMin.HasValue, e => e.Deduction >= deductionMin!.Value)
                    .WhereIf(deductionMax.HasValue, e => e.Deduction <= deductionMax!.Value)
                    .WhereIf(effectiveFromMin.HasValue, e => e.EffectiveFrom >= effectiveFromMin!.Value)
                    .WhereIf(effectiveFromMax.HasValue, e => e.EffectiveFrom <= effectiveFromMax!.Value)
                    .WhereIf(effectiveToMin.HasValue, e => e.EffectiveTo >= effectiveToMin!.Value)
                    .WhereIf(effectiveToMax.HasValue, e => e.EffectiveTo <= effectiveToMax!.Value)
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
            decimal? baseAmountMin = null,
            decimal? baseAmountMax = null,
            decimal? bonusMin = null,
            decimal? bonusMax = null,
            decimal? deductionMin = null,
            decimal? deductionMax = null,
            DateTime? effectiveFromMin = null,
            DateTime? effectiveFromMax = null,
            DateTime? effectiveToMin = null,
            DateTime? effectiveToMax = null,
            decimal? totalAmountMin = null,
            decimal? totalAmountMax = null,
            Guid? employeeId = null
            ) =>
                query
                    .WhereIf(baseAmountMin.HasValue, e => e.Salary.BaseAmount >= baseAmountMin!.Value)
                    .WhereIf(baseAmountMax.HasValue, e => e.Salary.BaseAmount <= baseAmountMax!.Value)
                    .WhereIf(bonusMin.HasValue, e => e.Salary.Bonus >= bonusMin!.Value)
                    .WhereIf(bonusMax.HasValue, e => e.Salary.Bonus <= bonusMax!.Value)
                    .WhereIf(deductionMin.HasValue, e => e.Salary.Deduction >= deductionMin!.Value)
                    .WhereIf(deductionMax.HasValue, e => e.Salary.Deduction <= deductionMax!.Value)
                    .WhereIf(effectiveFromMin.HasValue, e => e.Salary.EffectiveFrom >= effectiveFromMin!.Value)
                    .WhereIf(effectiveFromMax.HasValue, e => e.Salary.EffectiveFrom <= effectiveFromMax!.Value)
                    .WhereIf(effectiveToMin.HasValue, e => e.Salary.EffectiveTo >= effectiveToMin!.Value)
                    .WhereIf(effectiveToMax.HasValue, e => e.Salary.EffectiveTo <= effectiveToMax!.Value)
                    .WhereIf(employeeId.HasValue, e => e.Salary.EmployeeId == employeeId);


    }

    #endregion
}

