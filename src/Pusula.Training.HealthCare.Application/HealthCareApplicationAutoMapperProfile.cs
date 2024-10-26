using AutoMapper;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.Salaries;
using Pusula.Training.HealthCare.Shared;

using System;
using System.Numerics;


namespace Pusula.Training.HealthCare;

public class HealthCareApplicationAutoMapperProfile : Profile
{
    public HealthCareApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        

        CreateMap<Department, DepartmentDto>();
        CreateMap<Department, DepartmentExcelDto>();
        CreateMap<DepartmentDto, DepartmentUpdateDto>();
        CreateMap<Department, LookupDto<Guid>>().ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));

        CreateMap<Employee, EmployeeDto>();
        CreateMap<Employee, EmployeeExcelDto>();
        CreateMap<Employee, EmployeeWithNavigationPropertiesDto>();
        CreateMap<EmployeeWithNavigationProperties, EmployeeWithNavigationPropertiesDto>();

        CreateMap<Salary, SalaryDto>();
        CreateMap<Salary, SalaryExcelDto>();
        CreateMap<Salary, SalaryWithNavigationPropertiesDto>();
        CreateMap<SalaryWithNavigationProperties, SalaryWithNavigationPropertiesDto>();


    }
}
