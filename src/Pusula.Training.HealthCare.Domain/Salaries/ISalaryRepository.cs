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
            Guid? employeeId = null,
            CancellationToken cancellationToken = default);

        Task<SalaryWithNavigationProperties> GetWithNavigationPropertiesAsync(
            Guid id,
            CancellationToken cancellationToken = default);

        Task<List<SalaryWithNavigationProperties>> GetListWithNavigationPropertiesAsync(
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
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);


        Task<List<Salary>> GetListAsync(
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
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);

        Task<long> GetCountAsync(
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
            Guid? employeeId = null,
            string? sorting = null,
            int maxResultCount = int.MaxValue,
            int skipCount = 0,
            CancellationToken cancellationToken = default);
    }
}
