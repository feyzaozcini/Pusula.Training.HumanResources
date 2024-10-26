using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Employees
{
    public interface IEmployeeAppService : IApplicationService
    {
        Task<PagedResultDto<EmployeeWithNavigationPropertiesDto>> GetListAsync(GetEmployeeInput input);

        Task<EmployeeWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input);

        Task<EmployeeDto> GetAsync(Guid id);

        Task DeleteAsync(Guid id);

        Task<EmployeeDto> CreateAsync(EmployeeCreateDto input);

        Task<EmployeeDto> UpdateAsync(Guid id, EmployeeUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(EmployeeExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> employeeIds);

        Task DeleteAllAsync(GetEmployeeInput input);

        Task<Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}
