using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Employees
{
    public class EmployeeDownloadTokenCacheItem
    {
        public string Token { get; set; } = null!;

        public List<EmployeeWithNavigationPropertiesDto> EmployeeWithNavigationPropertiesDtos { get; set; } = null!;

        public int TotalCount { get; set; }
    }
}
