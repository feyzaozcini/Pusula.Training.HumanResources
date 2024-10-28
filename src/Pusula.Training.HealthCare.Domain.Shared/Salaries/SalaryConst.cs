using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Salaries
{
    public class SalaryConst
    {
        private const string DefaultSorting = "{0}BaseAmount asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Salary." : string.Empty);
        }

        public const int BaseAmountMinLength = 0;
        public const int BaseAmountMaxLength = 1000000;
        public const int BonusMinLength = 0;
        public const int BonusMaxLength = 1000000;
        public const int DeductionMinLength = 0;
        public const int DeductionMaxLength = 1000000;

    }
}
