﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Pusula.Training.HealthCare.Handlers
{
    public class EmployeeDepartmentEventHandler(ILogger<EmployeeDepartmentEventHandler> log) : ITransientDependency
    {

    }
    
}
