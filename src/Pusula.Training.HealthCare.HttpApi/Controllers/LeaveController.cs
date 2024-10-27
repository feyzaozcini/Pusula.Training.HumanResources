using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Leaves;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Leaves")]
    [Route("api/app/leaves")]
    public class LeaveController(ILeaveAppService leaveAppService) : HealthCareController, ILeaveAppService
    {
        [HttpGet]
        public virtual Task<PagedResultDto<LeaveWithNavigationPropertiesDto>> GetListAsync(GetLeavesInput input) => leaveAppService.GetListAsync(input);

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public virtual Task<LeaveWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) => leaveAppService.GetWithNavigationPropertiesAsync(id);

        [HttpGet]
        [Route("{id}")]
        public Task<LeaveDto> GetAsync(Guid id) => leaveAppService.GetAsync(id);

        [HttpGet]
        [Route("employee-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetEmployeeLookupAsync(LookupRequestDto input) => leaveAppService.GetEmployeeLookupAsync(input);

        [HttpPost]
        public Task<LeaveDto> CreateAsync(LeaveCreateDto input) => leaveAppService.CreateAsync(input);

        [HttpPut]
        public Task<LeaveDto> UpdateAsync(LeaveUpdateDto input) => leaveAppService.UpdateAsync(input);

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id) => leaveAppService.DeleteAsync(id);

        [HttpGet]
        [Route("as-excel-file")]
        public Task<IRemoteStreamContent> GetListAsExcelFileAsync(LeaveExcelDownloadDto input) => leaveAppService.GetListAsExcelFileAsync(input);

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => leaveAppService.GetDownloadTokenAsync();

        [HttpDelete]
        [Route("")]
        public Task DeleteByIdsAsync(List<Guid> leavesId) => doctorsAppService.DeleteByIdsAsync(doctorIds);

        [HttpDelete]
        [Route("all")]
        public Task DeleteAllAsync(GetLeavesInput input) => leaveAppService.DeleteAllAsync(input);


    }
}
