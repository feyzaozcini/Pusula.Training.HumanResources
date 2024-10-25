using Microsoft.Extensions.Logging;

using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Distributed;

namespace Pusula.Training.HealthCare.Handlers
{
    public class DoctorTitleAndDepartmentEventHandler(ILogger<DoctorTitleAndDepartmentEventHandler> log) :  ITransientDependency
    {
        
    }
}
