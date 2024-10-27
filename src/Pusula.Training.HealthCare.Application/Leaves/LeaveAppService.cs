using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Leaves
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.Leaves.Default)]
    public class LeaveAppService(ILevaeRepository levaeRepository,
        LeaveManager leaveManager,
        IDistributedCache<LeaveDownloadTokenCacheItem, string> downloadTokenCache,
        IEmployeeRepository employeeRepository,
        IDistributedEventBus distributedEventBus) : HealthCareAppService, ILeaveAppService
    {

        public virtual async Task<PagedResultDto<LeaveWithNavigationPropertiesDto>> GetListAsync(GetLeavesInput input)
        {
            var totalCount = await levaeRepository.GetCountAsync(input.FilterText,input.StartDate,input.EndDate,input.LeaveType,input.Description ,input.EmployeeId , input.Sorting, input.MaxResultCount, input.SkipCount);
            var items = await levaeRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.StartDate, input.EndDate, input.LeaveType, input.Description, input.EmployeeId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<LeaveWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<LeaveWithNavigationProperties>, List<LeaveWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<LeaveWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            var leave = await levaeRepository.GetWithNavigationPropertiesAsync(id);
            await distributedEventBus.PublishAsync(new LeaveEmployeeEto { Employee = leave.Employee.FirstName });
            return ObjectMapper.Map<LeaveWithNavigationProperties, LeaveWithNavigationPropertiesDto>(leave);
        }

        public virtual async Task<LeaveDto> GetAsync(Guid id) => ObjectMapper.Map<Leave, LeaveDto>(
                await levaeRepository.GetAsync(id));

        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetEmployeeLookupAsync(LookupRequestDto input)
        {
            var query = (await employeeRepository.GetQueryableAsync())
                           .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                               x => x.FirstName != null && x.FirstName.Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Employee>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Employee>, List<LookupDto<Guid>>>(lookupData)
            };
        }
        [Authorize(HealthCarePermissions.Leaves.Delete)]
        public virtual async Task DeleteAsync(Guid id) => await levaeRepository.DeleteAsync(id);

        [Authorize(HealthCarePermissions.Leaves.Create)]
        public virtual async Task<LeaveDto> CreateAsync(LeaveCreateDto input) => ObjectMapper.Map<Leave, LeaveDto>(
            await leaveManager.CreateAsync(input.EmployeeId, input.StartDate, input.EndDate, input.LeaveType, input.Description));

        [Authorize(HealthCarePermissions.Leaves.Edit)]
        public virtual async Task<LeaveDto> UpdateAsync(LeaveUpdateDto input) => ObjectMapper.Map<Leave, LeaveDto>(
            await leaveManager.UpdateAsync(input.Id, input.EmployeeId, input.StartDate, input.EndDate, input.LeaveType, input.Description));


        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(LeaveExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var leaves = await levaeRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.StartDate, input.EndDate, input.LeaveType, input.Description, input.EmployeeId);
            var items = leaves.Select(item => new LeaveExcelDto
            {
                StartDate = item.Leave.StartDate,
                EndDate = item.Leave.EndDate,
                LeaveType = item.Leave.LeaveType,
                Description = item.Leave.Description,
                Employee = item.Employee.FirstName ?? string.Empty,
            });


            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Leaves.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(HealthCarePermissions.Leaves.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> leavesId) => await levaeRepository.DeleteManyAsync(leavesId);

        [Authorize(HealthCarePermissions.Leaves.Delete)]
        public virtual async Task DeleteAllAsync(GetLeavesInput input) => await levaeRepository.DeleteAllAsync(input.FilterText, input.StartDate, input.EndDate, input.LeaveType, input.Description, input.EmployeeId);



        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await downloadTokenCache.SetAsync(
                token,
                new LeaveDownloadTokenCacheItem { Token = token },
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
                });

            return new Shared.DownloadTokenResultDto
            {
                Token = token
            };
        }

        
        
        

        

        
    }
}
