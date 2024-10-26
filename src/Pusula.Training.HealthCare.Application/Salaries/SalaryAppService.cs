using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization;
using Volo.Abp.Caching;
using Volo.Abp.Content;
using Volo.Abp.EventBus.Distributed;


namespace Pusula.Training.HealthCare.Salaries
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.Salaries.Default)]
    public class SalaryAppService(
        ISalaryRepository salaryRepository,
        SalaryManager salaryManager,
        IDistributedCache<SalaryDownloadTokenCacheItem, string> downloadTokenCache,
        IDistributedEventBus distributedEventBus,
        IEmployeeRepository employeeRepository

        ) : HealthCareAppService, ISalaryAppService
    {
        public virtual async Task<PagedResultDto<SalaryWithNavigationPropertiesDto>> GetListAsync(GetSalaryInput input)
        {
            var totalCount = await salaryRepository.GetCountAsync(input.BaseAmountMin, input.BaseAmountMax, input.BonusMin, input.BonusMax, input.DeductionMin, input.DeductionMax, input.EffectiveFromMin,
                input.EffectiveFromMax, input.EffectiveToMin, input.EffectiveToMax, input.TotalAmountMin, input.TotalAmountMax,input.EmployeeId, input.Sorting, input.MaxResultCount, input.SkipCount);

            var items = await salaryRepository.GetListWithNavigationPropertiesAsync(input.BaseAmountMin, input.BaseAmountMax, input.BonusMin, input.BonusMax, input.DeductionMin, input.DeductionMax, input.EffectiveFromMin,
                input.EffectiveFromMax, input.EffectiveToMin, input.EffectiveToMax, input.TotalAmountMin, input.TotalAmountMax, input.EmployeeId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<SalaryWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<SalaryWithNavigationProperties>, List<SalaryWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<SalaryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            var salary = await salaryRepository.GetWithNavigationPropertiesAsync(id);
            await distributedEventBus.PublishAsync(new SalaryEmployeeEto { Employee = salary.Employee.FirstName });
            return ObjectMapper.Map<SalaryWithNavigationProperties, SalaryWithNavigationPropertiesDto>(salary);
        }

        public virtual async Task<SalaryDto> GetAsync(Guid id) => ObjectMapper.Map<Salary, SalaryDto>(
                await salaryRepository.GetAsync(id));

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

        [Authorize(HealthCarePermissions.Salaries.Create)]
        public virtual async Task<SalaryDto> CreateAsync(SalaryCreateDto input) => ObjectMapper.Map<Salary, SalaryDto>(
                await salaryManager.CreateAsync(
                     input.EmployeeId, input.BaseAmount,input.Bonus,input.Deduction,input.EffectiveFrom,input.EffectiveTo,input.TotalAmount));

        [Authorize(HealthCarePermissions.Salaries.Edit)]
        public virtual async Task<SalaryDto> UpdateAsync(Guid id, SalaryUpdateDto input) => ObjectMapper.Map<Salary, SalaryDto>(
            await salaryManager.UpdateAsync(input.Id, input.EmployeeId, input.BaseAmount, input.Bonus, input.Deduction, input.EffectiveFrom, input.EffectiveTo, input.TotalAmount));

        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(SalaryExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var salaries = await salaryRepository.GetListWithNavigationPropertiesAsync(input.BaseAmountMin, input.BaseAmountMax, input.BonusMin, input.BonusMax, input.DeductionMin, input.DeductionMax, input.EffectiveFromMin,
                input.EffectiveFromMax, input.EffectiveToMin, input.EffectiveToMax, input.TotalAmountMin, input.TotalAmountMax, input.EmployeeId);
            var items = salaries.Select(item => new SalaryExcelDto
            {
                BaseAmount = item.Salary.BaseAmount,
                EffectiveFrom = item.Salary.EffectiveFrom

            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Salaries.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(HealthCarePermissions.Salaries.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> salaryIds) => await salaryRepository.DeleteManyAsync(salaryIds);

        [Authorize(HealthCarePermissions.Salaries.Delete)]
        public virtual async Task DeleteAllAsync(GetSalaryInput input) => await salaryRepository.DeleteAllAsync(input.BaseAmountMin, input.BaseAmountMax, input.BonusMin, input.BonusMax, input.DeductionMin, input.DeductionMax, input.EffectiveFromMin,
                input.EffectiveFromMax, input.EffectiveToMin, input.EffectiveToMax, input.TotalAmountMin, input.TotalAmountMax, input.EmployeeId);

        [Authorize(HealthCarePermissions.Salaries.Delete)]
        public virtual async Task DeleteAsync(Guid id) => await salaryRepository.DeleteAsync(id);


        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await downloadTokenCache.SetAsync(
                token,
                new SalaryDownloadTokenCacheItem { Token = token },
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
