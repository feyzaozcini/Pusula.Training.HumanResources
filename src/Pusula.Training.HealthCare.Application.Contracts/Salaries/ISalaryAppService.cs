using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Salaries
{
    public interface ISalaryAppService : IApplicationService
    {
        Task<PagedResultDto<SalaryWithNavigationPropertiesDto>> GetListAsync(GetSalaryInput input);

        Task<SalaryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetEmployeeLookupAsync(LookupRequestDto input);

        Task<SalaryDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<SalaryDto> CreateAsync(SalaryCreateDto input);

        Task<SalaryDto> UpdateAsync(Guid id, SalaryUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(SalaryExcelDownloadDto input);

        Task DeleteByIdsAsync(List<Guid> salaryIds);

        Task DeleteAllAsync(GetSalaryInput input);

        Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}
