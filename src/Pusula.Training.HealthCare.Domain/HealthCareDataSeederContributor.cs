using Microsoft.AspNetCore.Identity;
using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.Leaves;
using Pusula.Training.HealthCare.Permissions;
using Pusula.Training.HealthCare.Salaries;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Users;
using static Volo.Abp.Identity.IdentityPermissions;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace Pusula.Training.HealthCare
{
    public class HealthCareDataSeederContributor(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository, ISalaryRepository salaryRepository, ILeaveRepository leaveRepository,

        IGuidGenerator guidGenerator, IdentityUserManager userManager,
            IdentityRoleManager roleManager, IPermissionManager permissionManager) : IDataSeedContributor, ITransientDependency
    {
        public async Task SeedAsync(DataSeedContext context)
        {

            var department = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "Bilgi Teknolojileri"), true);
            var department2 = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "İnsan Kaynakları"), true);
            var department3 = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "Dış Ticaret"), true);

            var employee=await employeeRepository.InsertAsync(new Employee(guidGenerator.Create(), department.Id,"Ahmet", "Yılmaz", "14454540578", new System.DateTime(1990, 05, 20), "feyza@gmail.com","5353771043",EnumGender.Female), true);

            var leave = await leaveRepository.InsertAsync(new Leave(guidGenerator.Create(), employee.Id, new System.DateTime(2021, 05, 20), new System.DateTime(2021, 05, 25), LeaveType.AnnualLeave, "Yıllık izin"), true);

            var salary= await salaryRepository.InsertAsync(new Salary(guidGenerator.Create(), employee.Id, 5000, 1000, 500, new System.DateTime(2021, 05, 20), new System.DateTime(2021, 05, 25)), true);

            //Create Roles
            var role= await roleManager.FindByNameAsync("HumanResources");
            if (role == null)
            {
                role = new IdentityRole(guidGenerator.Create(), "HumanResources");
                await roleManager.CreateAsync(role);
            }

            var user = new IdentityUser(guidGenerator.Create(), "hruser", "hruser@gmail.com");

            // Kullanıcıyı parola ile oluşturma
            var result = await userManager.CreateAsync(user, "Human.123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "HumanResources");
                await userManager.AddClaimAsync(user, new Claim("HumanResources", "true"));

                await userManager.AddClaimAsync(user, new Claim("HumanResources", "true"));
            }

            
        }
    }
}
