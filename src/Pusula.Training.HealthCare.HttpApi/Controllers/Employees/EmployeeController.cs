using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Employees
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Employee")]
    [Route("api/app/employees")]
    public class EmployeeController(IEmployeeAppService employeeAppService) : HealthCareController, IEmployeeAppService
    {
        [HttpGet]
        public Task<PagedResultDto<EmployeeWithNavigationPropertiesDto>> GetListAsync(GetEmployeeInput input) => employeeAppService.GetListAsync(input);

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<EmployeeWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) => employeeAppService.GetWithNavigationPropertiesAsync(id);

        [HttpGet]
        [Route("{id}")]
        public Task<EmployeeDto> GetAsync(Guid id) => employeeAppService.GetAsync(id);

        [HttpGet]
        [Route("department-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetDepartmentLookupAsync(LookupRequestDto input) => employeeAppService.GetDepartmentLookupAsync(input);


        [HttpPost]
        public Task<EmployeeDto> CreateAsync(EmployeeCreateDto input) => employeeAppService.CreateAsync(input);

        [HttpPut]
        public Task<EmployeeDto> UpdateAsync(Guid id, EmployeeUpdateDto input) => employeeAppService.UpdateAsync(id, input);

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id) => employeeAppService.DeleteAsync(id);

        [HttpGet]
        [Route("as-excel-file")]
        public Task<IRemoteStreamContent> GetListAsExcelFileAsync(EmployeeExcelDownloadDto input) => employeeAppService.GetListAsExcelFileAsync(input);

        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => employeeAppService.GetDownloadTokenAsync();

        [HttpDelete]
        [Route("")]
        public Task DeleteByIdsAsync(List<Guid> employeeIds) => employeeAppService.DeleteByIdsAsync(employeeIds);

        [HttpDelete]
        [Route("all")]
        public Task DeleteAllAsync(GetEmployeeInput input) => employeeAppService.DeleteAllAsync(input);



    }
}
