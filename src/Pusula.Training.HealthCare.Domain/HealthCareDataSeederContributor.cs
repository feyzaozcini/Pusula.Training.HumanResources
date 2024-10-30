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
            var department4 = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "Yönetim"), true);

            var employee=await employeeRepository.InsertAsync(new Employee(guidGenerator.Create(), department.Id,"Ahmet", "Yılmaz", "14444444442", new System.DateTime(1990, 05, 20), "ahmet@gmail.com","5355353535",EnumGender.Male), true);
            var employee2 = await employeeRepository.InsertAsync(new Employee(guidGenerator.Create(), department2.Id, "Merve", "Ateş", "1444444444", new System.DateTime(1995, 06, 15), "merve@gmail.com", "5353443434", EnumGender.Female), true);
            var employee3 = await employeeRepository.InsertAsync(new Employee(guidGenerator.Create(), department3.Id, "Ayşe", "Kaya", "14444444446", new System.DateTime(1992, 10, 20), "ayse@gmail.com", "53533533635", EnumGender.Female), true);
            var employee4 = await employeeRepository.InsertAsync(new Employee(guidGenerator.Create(), department4.Id, "Berke", "Demir", "1444444448", new System.DateTime(1997, 03, 05), "berke@gmail.com", "5353448575", EnumGender.Male), true);


            var leave = await leaveRepository.InsertAsync(new Leave(guidGenerator.Create(), employee.Id, new System.DateTime(2021, 05, 20), new System.DateTime(2021, 05, 25), LeaveType.AnnualLeave, "Yıllık izin"), true);
            var leave2 = await leaveRepository.InsertAsync(new Leave(guidGenerator.Create(), employee2.Id, new System.DateTime(2022, 06, 20), new System.DateTime(2022, 06, 23), LeaveType.SickLeave, "Hasta"), true);
            var leave3 = await leaveRepository.InsertAsync(new Leave(guidGenerator.Create(), employee3.Id, new System.DateTime(2024, 05, 20), new System.DateTime(2024, 05, 29), LeaveType.UnpaidLeave, "Ücretli izin"), true);
            var leave4 = await leaveRepository.InsertAsync(new Leave(guidGenerator.Create(), employee4.Id, new System.DateTime(2024, 07, 20), new System.DateTime(2024, 07, 25), LeaveType.MaternityLeave, "Yıllık izin"), true);

            var salary= await salaryRepository.InsertAsync(new Salary(guidGenerator.Create(), employee.Id, 5000, 1000, 500, new System.DateTime(2021, 05, 20), new System.DateTime(2021, 12, 20)), true);
            var salary2 = await salaryRepository.InsertAsync(new Salary(guidGenerator.Create(), employee2.Id, 6000, 2000, 1000, new System.DateTime(2022, 06, 20), new System.DateTime(2023, 06, 23)), true);
            var salary3 = await salaryRepository.InsertAsync(new Salary(guidGenerator.Create(), employee3.Id, 7000, 3000, 1500, new System.DateTime(2024, 05, 20), new System.DateTime(2025, 05, 20)), true);
            var salary4 = await salaryRepository.InsertAsync(new Salary(guidGenerator.Create(), employee4.Id, 8000, 4000, 2000, new System.DateTime(2024, 07, 20), new System.DateTime(2025, 01, 20)), true);

            //Create Roles
            var role= await roleManager.FindByNameAsync("hruser");
            if (role == null)
            {
                role = new IdentityRole(guidGenerator.Create(), "hruser");

                await roleManager.CreateAsync(role);
                
            }

            await roleManager.AddClaimAsync(role, new Claim(HealthCarePermissions.Dashboard.DashboardGroup, "true"));
            await roleManager.AddClaimAsync(role, new Claim(HealthCarePermissions.Departments.Default, "true"));
            await roleManager.AddClaimAsync(role, new Claim(HealthCarePermissions.Departments.Create, "true"));
            await roleManager.AddClaimAsync(role, new Claim(HealthCarePermissions.Salaries.Default, "true"));
            await roleManager.AddClaimAsync(role, new Claim(HealthCarePermissions.Salaries.Create, "true"));
            await roleManager.AddClaimAsync(role, new Claim(HealthCarePermissions.Employees.Default, "true"));
            await roleManager.AddClaimAsync(role, new Claim(HealthCarePermissions.Employees.Create, "true"));
            await roleManager.AddClaimAsync(role, new Claim(HealthCarePermissions.Leaves.Default, "true"));
            await roleManager.AddClaimAsync(role, new Claim(HealthCarePermissions.Leaves.Create, "true"));

            var user = new IdentityUser(guidGenerator.Create(), "hruser", "hruser@gmail.com");

            // Kullanıcıyı parola ile oluşturma
            var result = await userManager.CreateAsync(user, "Human.123");

            await userManager.AddToRoleAsync(user, "hruser");
            


        }
    }
}
