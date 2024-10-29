using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.EventBus.Distributed;
using Pusula.Training.HealthCare.Employees;

namespace Pusula.Training.HealthCare.Handlers
{
    public class EmployeeDepartmentEventHandler(ILogger<EmployeeDepartmentEventHandler> log) : IDistributedEventHandler<EmployeeDepartmentEto>, ITransientDependency
    {
        public Task HandleEventAsync(EmployeeDepartmentEto eventData)
        {
            log.LogInformation($" -----> HANDLER ->  Employee  in {eventData.Department} department.");
            return Task.CompletedTask;
        }
    }

}
