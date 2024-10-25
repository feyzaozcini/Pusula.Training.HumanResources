using Pusula.Training.HealthCare.Departments;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;

namespace Pusula.Training.HealthCare
{
    public class HealthCareDataSeederContributor(IDepartmentRepository departmentRepository,
        
        IGuidGenerator guidGenerator) : IDataSeedContributor, ITransientDependency
    {
        public async Task SeedAsync(DataSeedContext context)
        {

            var department = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "Kulak Burun Boğaz"), true);
            var department2 = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "Dermatoloji"), true);
            var department3 = await departmentRepository.InsertAsync(new Department(guidGenerator.Create(), "Dahiliye"), true);

           
        }
    }
}
