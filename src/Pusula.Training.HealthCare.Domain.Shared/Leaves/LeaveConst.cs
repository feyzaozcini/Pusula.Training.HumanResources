using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pusula.Training.HealthCare.Leaves
{
    public class LeaveConst
    {
        private const string DefaultSorting = "{0}StartDate asc";

        public static string GetDefaultSorting(bool withEntityName)
        {
            return string.Format(DefaultSorting, withEntityName ? "Leave." : string.Empty);
        }
    }
}
