using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Pusula.Training.HealthCare.Salaries
{
    public interface ISalaryRepository : IRepository<Salary, Guid>
    {
        Task DeleteAllAsync(
            decimal? baseAmount = null,
            decimal? bonus = null,
            decimal? deduction = null,
            DateTime? effectiveFrom = null,
            DateTime? effectiveTo = null,
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<SalaryWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<SalaryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
            decimal? baseAmount = null,
            decimal? bonus = null,
            decimal? deduction = null,
            DateTime? effectiveFrom = null,
            DateTime? effectiveTo = null,
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);


        Task<List<Salary>> GetListAsync(
            decimal? baseAmount = null,
            decimal? bonus = null,
            decimal? deduction = null,
            DateTime? effectiveFrom = null,
            DateTime? effectiveTo = null,
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
            decimal? baseAmount = null,
            decimal? bonus = null,
            decimal? deduction = null,
            DateTime? effectiveFrom = null,
            DateTime? effectiveTo = null,
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
    }
}
