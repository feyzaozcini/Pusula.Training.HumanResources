using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Leaves
{
    public interface ILeaveAppService : IApplicationService
    {
        Task<PagedResultDto<LeaveWithNavigationPropertiesDto>> GetListAsync(GetLeavesInput input);

        Task<LeaveWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id);

        Task<LeaveDto> GetAsync(Guid id);

        Task<PagedResultDto<LookupDto<Guid>>> GetEmployeeLookupAsync(LookupRequestDto input);

        Task DeleteAsync(Guid id);

        Task<LeaveDto> CreateAsync(LeaveCreateDto input);

        Task<LeaveDto> UpdateAsync(LeaveUpdateDto input);

        Task<IRemoteStreamContent> GetListAsExcelFileAsync(LeaveExcelDownloadDto input);
        Task DeleteByIdsAsync(List<Guid> leavesId);

        Task DeleteAllAsync(GetLeavesInput input);
        Task<Pusula.Training.HealthCare.Shared.DownloadTokenResultDto> GetDownloadTokenAsync();
    }
}
