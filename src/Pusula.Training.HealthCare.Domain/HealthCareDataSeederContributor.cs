using Pusula.Training.HealthCare.Departments;
using Pusula.Training.HealthCare.Employees;
using Pusula.Training.HealthCare.Salaries;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Pusula.Training.HealthCare
{
    public class HealthCareDataSeederContributor(IDepartmentRepository departmentRepository, IEmployeeRepository employeeRepository, ISalaryRepository salaryRepository,

        IGuidGenerator guidGenerator) : IDataSeedContributor, ITransientDependency
    {
        public async Task SeedAsync(DataSeedContext context)
        {

            var department = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "Bilgi Teknolojileri"), true);
            var department2 = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "İnsan Kaynakları"), true);
            var department3 = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "Dış Ticaret"), true);

            var employee=await employeeRepository.InsertAsync(new Employee(guidGenerator.Create(), department.Id,"Ahmet", "Yılmaz", "14454540578", new System.DateTime(1990, 05, 20), "feyza@gmail.com","5353771043",EnumGender.Female), true);

            

        }
    }
}
