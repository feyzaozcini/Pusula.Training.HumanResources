using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using MiniExcelLibs;
using Pusula.Training.HealthCare.Departments;
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
using Volo.Abp.ObjectMapping;

namespace Pusula.Training.HealthCare.Employees
{
    [RemoteService(IsEnabled = false)]
    [Authorize(HealthCarePermissions.Employees.Default)]
    [Authorize(Roles = "HumanResources")]
    public class EmployeeAppService(
        IEmployeeRepository employeeRepository,
        EmployeeManager employeeManager, 
        IDistributedCache<EmployeeDownloadTokenCacheItem, string> downloadTokenCache,
        IDistributedEventBus distributedEventBus,
        IDepartmentRepository departmentRepository

        ) : HealthCareAppService, IEmployeeAppService
    {

        public virtual async Task<PagedResultDto<EmployeeWithNavigationPropertiesDto>> GetListAsync(GetEmployeeInput input)
        {
            var totalCount = await employeeRepository.GetCountAsync(input.FilterText, input.FirstName, input.LastName, input.IdentityNumber ,input.BirthDateMin,input.BirthDateMax,input.Email,
                input.MobilePhoneNumber,input.Gender,input.DepartmentId, input.Sorting,input.MaxResultCount,input.SkipCount);

            var items=await employeeRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDateMin, input.BirthDateMax, input.Email,
                input.MobilePhoneNumber, input.Gender, input.DepartmentId, input.Sorting, input.MaxResultCount, input.SkipCount);

            return new PagedResultDto<EmployeeWithNavigationPropertiesDto>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<EmployeeWithNavigationProperties>, List<EmployeeWithNavigationPropertiesDto>>(items)
            };
        }

        public virtual async Task<EmployeeWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id)
        {
            var employee = await employeeRepository.GetWithNavigationPropertiesAsync(id);
            await distributedEventBus.PublishAsync(new EmployeeDepartmentEto { Department = employee.Department.Name});
            return ObjectMapper.Map<EmployeeWithNavigationProperties, EmployeeWithNavigationPropertiesDto>(employee);
        }

        public virtual async Task<EmployeeDto> GetAsync(Guid id) => ObjectMapper.Map<Employee, EmployeeDto>(
                await employeeRepository.GetAsync(id));


        public virtual async Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input)
        {
            var query = (await departmentRepository.GetQueryableAsync())
                .WhereIf(!string.IsNullOrWhiteSpace(input.Filter),
                    x => x.Name != null && x.Name.Contains(input.Filter!));

            var lookupData = await query.PageBy(input.SkipCount, input.MaxResultCount).ToDynamicListAsync<Department>();
            var totalCount = query.Count();
            return new PagedResultDto<LookupDto<Guid>>
            {
                TotalCount = totalCount,
                Items = ObjectMapper.Map<List<Department>, List<LookupDto<Guid>>>(lookupData)
            };
        }


        [Authorize(HealthCarePermissions.Employees.Create)]
        [Authorize(Roles = "HumanResources")]
        public virtual async Task<EmployeeDto> CreateAsync(EmployeeCreateDto input) => ObjectMapper.Map<Employee, EmployeeDto>(
                await employeeManager.CreateAsync(
                    input.DepartmentId, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDate, input.Email, input.MobilePhoneNumber, input.Gender));


        [Authorize(HealthCarePermissions.Employees.Edit)]
        public virtual async Task<EmployeeDto> UpdateAsync(Guid id, EmployeeUpdateDto input) => ObjectMapper.Map<Employee, EmployeeDto>(
                await employeeManager.UpdateAsync(
                    input.Id, input.DepartmentId, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDate, input.Email, input.MobilePhoneNumber));



        [AllowAnonymous]
        public virtual async Task<IRemoteStreamContent> GetListAsExcelFileAsync(EmployeeExcelDownloadDto input)
        {
            var downloadToken = await downloadTokenCache.GetAsync(input.DownloadToken);
            if (downloadToken == null || input.DownloadToken != downloadToken.Token)
            {
                throw new AbpAuthorizationException("Invalid download token: " + input.DownloadToken);
            }

            var employees = await employeeRepository.GetListWithNavigationPropertiesAsync(input.FilterText, input.FirstName, input.LastName,input.IdentityNumber,input.BirthDateMin,input.BirthDateMax,input.Email,input.MobilePhoneNumber,input.Gender,input.DepartmentId);
            var items = employees.Select(item => new EmployeeExcelDto
            {
                FirstName = item.Employee.FirstName,
                LastName = item.Employee.LastName,
                IdentityNumber = item.Employee.IdentityNumber,
                BirthDate = item.Employee.BirthDate,
                Email = item.Employee.Email,
                MobilePhoneNumber = item.Employee.MobilePhoneNumber,
                Gender = item.Employee.Gender,
                Department = item.Department?.Name ?? string.Empty,
                
            });

            var memoryStream = new MemoryStream();
            await memoryStream.SaveAsAsync(items);
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new RemoteStreamContent(memoryStream, "Employess.xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [Authorize(HealthCarePermissions.Employees.Delete)]
        public virtual async Task DeleteByIdsAsync(List<Guid> employeeIds) => await employeeRepository.DeleteManyAsync(employeeIds);


        [Authorize(HealthCarePermissions.Employees.Delete)]
        public virtual async Task DeleteAllAsync(GetEmployeeInput input) => await employeeRepository.DeleteAllAsync(input.FilterText, input.FirstName, input.LastName, input.IdentityNumber, input.BirthDateMin, input.BirthDateMax, input.Email, input.MobilePhoneNumber, input.Gender, input.DepartmentId);


        [Authorize(HealthCarePermissions.Employees.Delete)]
        public virtual async Task DeleteAsync(Guid id) => await employeeRepository.DeleteAsync(id);



        public virtual async Task<DownloadTokenResultDto> GetDownloadTokenAsync()
        {
            var token = Guid.NewGuid().ToString("N");

            await downloadTokenCache.SetAsync(
                token,
                new EmployeeDownloadTokenCacheItem { Token = token },
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
