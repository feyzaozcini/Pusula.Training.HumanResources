using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Employees
{
    public static class EmployeeConst
    {
        private const string DefaultSorting = "{0}FirstName asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Employee." : string.Empty);
        }

        public const int FirstNameMinLength = 2;
        public const int FirstNameMaxLength = 128;
        public const int LastNameMinLength = 2;
        public const int LastNameMaxLength = 128;
        public const int IdentityNumberMaxLength = 11;
        public const int EmailMaxLength = 128;
        public const int MobilePhoneNumberMaxLength = 32;

    }
}
