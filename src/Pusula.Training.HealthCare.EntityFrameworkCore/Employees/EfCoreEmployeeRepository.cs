using Microsoft.EntityFrameworkCore;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace Pusula.Training.HealthCare.Employees
{
    public class EfCoreEmployeeRepository(IDbContextProvider<HealthCareDbContext> dbContextProvider)
        : EfCoreRepository<HealthCareDbContext, Employee, Guid>(dbContextProvider), IEmployeeRepository
    {
        public virtual async Task DeleteAllAsync(
            string? filterText = null, 
            string? firstName = null, 
            string? lastName = null, 
            string? identityNumber = null, 
            DateTime? birthDateMin = null, 
            DateTime? birthDateMax = null, 
            string? email = null, 
            string? mobilePhoneNumber = null, 
            EnumGender? gender = null, 
            Guid? departmentId = null, 
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, birthDateMax, birthDateMin,email,mobilePhoneNumber,gender,departmentId);

            var ids = query.Select(x => x.Employee.Id);
            await DeleteManyAsync(ids, cancellationToken: GetCancellationToken(cancellationToken));
        }

        public virtual async Task<long> GetCountAsync(
            string? filterText = null, 
            string? firstName = null, string? 
            lastName = null, string? 
            identityNumber = null, 
            DateTime? birthDateMin = null, 
            DateTime? birthDateMax = null, 
            string? email = null, string? 
            mobilePhoneNumber = null, 
            EnumGender? gender = null, 
            Guid? departmentId = null, 
            string? sorting = null, 
            int maxResultCount = int.MaxValue, 
            int skipCount = 0, 
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, birthDateMax, birthDateMin, email, mobilePhoneNumber, gender, departmentId);
            return await query.LongCountAsync(GetCancellationToken(cancellationToken));
        }

        public virtual async Task<List<Employee>> GetListAsync(
            string? filterText = null, 
            string? firstName = null, 
            string? lastName = null, 
            string? identityNumber = null, 
            DateTime? birthDateMin = null, 
            DateTime? birthDateMax = null, 
            string? email = null, 
            string? mobilePhoneNumber = null, 
            EnumGender? gender = null, 
            Guid? departmentId = null, 
            string? sorting = null, 
            int maxResultCount = int.MaxValue, 
            int skipCount = 0, 
            CancellationToken cancellationToken = default)
        {
            var query = ApplyFilter(await GetQueryableAsync(), filterText, firstName, lastName, identityNumber, birthDateMax, birthDateMin, email, mobilePhoneNumber, gender, departmentId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? EmployeeConst.GetDefaultSorting(false) : sorting);
            return await query.Page(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<List<EmployeeWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            string? filterText = null, 
            string? firstName = null, 
            string? lastName = null, 
            string? identityNumber = null, 
            DateTime? birthDateMin = null, 
            DateTime? birthDateMax = null, 
            string? email = null, 
            string? mobilePhoneNumber = null, 
            EnumGender? gender = null, 
            Guid? departmentId = null, 
            string? sorting = null, 
            int maxResultCount = int.MaxValue, 
            int skipCount = 0, 
            CancellationToken cancellationToken = default)
        {
            var query = await GetQueryForNavigationPropertiesAsync();
            query = ApplyFilter(query, filterText, firstName, lastName, identityNumber, birthDateMax, birthDateMin, email, mobilePhoneNumber, gender, departmentId);
            query = query.OrderBy(string.IsNullOrWhiteSpace(sorting) ? EmployeeConst.GetDefaultSorting(true) : sorting);
            return await query.PageBy(skipCount, maxResultCount).ToListAsync(cancellationToken);
        }

        public virtual async Task<EmployeeWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id, 
            CancellationToken cancellationToken = default)
        {
            var dbContext = await GetDbContextAsync();

            return (await GetDbSetAsync()).Where(b => b.Id == id)
                .Select(employee => new EmployeeWithNavigationProperties
                {
                    Employee = employee,
                    Department = dbContext.Set<Department>().FirstOrDefault(c => c.Id == employee.DepartmentId)!
                })
                .FirstOrDefault()!;
        }

        #region ApplyFilter and Queryable
        protected virtual IQueryable<Employee> ApplyFilter(
            IQueryable<Employee> query,
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            string? identityNumber = null,
            DateTime? birthDateMin = null,
            DateTime? birthDateMax = null,
            string? email = null,
            string? mobilePhoneNumber = null,
            EnumGender? gender = null,
            Guid? departmentId = null ) =>
                query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.FirstName!.Contains(filterText!) || e.LastName!.Contains(filterText!) || e.MobilePhoneNumber!.Contains(filterText!) || e.IdentityNumber!.Contains(filterText!) || e.Email!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.FirstName!.Contains(firstName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.LastName!.Contains(lastName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(mobilePhoneNumber), e => e.MobilePhoneNumber!.Contains(mobilePhoneNumber!))
                    .WhereIf(birthDateMin.HasValue, e => e.BirthDate >= birthDateMin!.Value)
                    .WhereIf(birthDateMax.HasValue, e => e.BirthDate <= birthDateMax!.Value)
                    .WhereIf(gender.HasValue, e => e.Gender == gender)
                    .WhereIf(departmentId.HasValue, e => e.DepartmentId == departmentId);
                    

        protected virtual async Task<IQueryable<EmployeeWithNavigationProperties>> GetQueryForNavigationPropertiesAsync() =>
            from employee in (await GetDbSetAsync())
            join department in (await GetDbContextAsync()).Set<Department>() on employee.DepartmentId equals department.Id into departments
            from department in departments.DefaultIfEmpty()
            select new EmployeeWithNavigationProperties
            {
                Employee = employee,
                Department = department
            };


        protected virtual IQueryable<EmployeeWithNavigationProperties> ApplyFilter(
            IQueryable<EmployeeWithNavigationProperties> query,
            string? filterText = null,
            string? firstName = null,
            string? lastName = null,
            string? identityNumber = null,
            DateTime? birthDateMin = null,
            DateTime? birthDateMax = null,
            string? email = null,
            string? mobilePhoneNumber = null,
            EnumGender? gender = null,
            Guid? departmentId = null
            ) =>
                query
                    .WhereIf(!string.IsNullOrWhiteSpace(filterText), e => e.Employee.FirstName!.Contains(filterText!) || e.Employee.LastName!.Contains(filterText!) || e.Employee.MobilePhoneNumber!.Contains(filterText!) || e.Employee.IdentityNumber!.Contains(filterText!) || e.Employee.Email!.Contains(filterText!))
                    .WhereIf(!string.IsNullOrWhiteSpace(firstName), e => e.Employee.FirstName!.Contains(firstName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(lastName), e => e.Employee.LastName!.Contains(lastName!))
                    .WhereIf(!string.IsNullOrWhiteSpace(identityNumber), e => e.Employee.IdentityNumber!.Contains(identityNumber!))
                    .WhereIf(!string.IsNullOrWhiteSpace(email), e => e.Employee.Email!.Contains(email!))
                    .WhereIf(!string.IsNullOrWhiteSpace(mobilePhoneNumber), e => e.Employee.MobilePhoneNumber!.Contains(mobilePhoneNumber!))
                    .WhereIf(birthDateMin.HasValue, e => e.Employee.BirthDate >= birthDateMin!.Value)
                    .WhereIf(birthDateMax.HasValue, e => e.Employee.BirthDate <= birthDateMax!.Value)
                    .WhereIf(gender.HasValue, e => e.Employee.Gender == gender)
                    .WhereIf(departmentId.HasValue, e => e.Department.Id == departmentId);
                
        #endregion
    }
}
