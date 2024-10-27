﻿using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Pusula.Training.HealthCare.Salaries;
using Pusula.Training.HealthCare.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Content;

namespace Pusula.Training.HealthCare.Controllers.Salaries
{
    [RemoteService]
    [Area("app")]
    [ControllerName("Salary")]
    [Route("api/app/salaries")]
    public class SalaryController(ISalaryAppService salaryAppService) : HealthCareController, ISalaryAppService
    {
        [HttpGet]
        public Task<PagedResultDto<SalaryWithNavigationPropertiesDto>> GetListAsync(GetSalaryInput input) => salaryAppService.GetListAsync(input);

        [HttpGet]
        [Route("with-navigation-properties/{id}")]
        public Task<SalaryWithNavigationPropertiesDto> GetWithNavigationPropertiesAsync(Guid id) => salaryAppService.GetWithNavigationPropertiesAsync(id);

        [HttpGet]
        [Route("{id}")]
        public Task<SalaryDto> GetAsync(Guid id) => salaryAppService.GetAsync(id);

        [HttpGet]
        [Route("employee-lookup")]
        public Task<PagedResultDto<LookupDto<Guid>>> GetEmployeeLookupAsync(LookupRequestDto input) => salaryAppService.GetEmployeeLookupAsync(input);


        [HttpPost]
        public Task<SalaryDto> CreateAsync(SalaryCreateDto input) => salaryAppService.CreateAsync(input);

        [HttpPut]
        public Task<SalaryDto> UpdateAsync(Guid id, SalaryUpdateDto input) => salaryAppService.UpdateAsync(id, input);

        [HttpDelete]
        [Route("{id}")]
        public Task DeleteAsync(Guid id) => salaryAppService.DeleteAsync(id);

        [HttpDelete]
        [Route("all")]
        public Task DeleteAllAsync(GetSalaryInput input) => salaryAppService.DeleteAllAsync(input);

        [HttpDelete]
        [Route("")]
        public Task DeleteByIdsAsync(List<Guid> salaryIds) => salaryAppService.DeleteByIdsAsync(salaryIds);


        [HttpGet]
        [Route("download-token")]
        public Task<DownloadTokenResultDto> GetDownloadTokenAsync() => salaryAppService.GetDownloadTokenAsync();

        [HttpGet]
        [Route("as-excel-file")]
        public Task<IRemoteStreamContent> GetListAsExcelFileAsync(SalaryExcelDownloadDto input) => salaryAppService.GetListAsExcelFileAsync(input);


    }
}