using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Helpers
{
    public class enumModel
    {
        public enum SearchCriteria
        {
            IsActiveDateFromAndIsActiveUpdatedDate,
            IsActiveDateFrom,
            IsActiveUpdatedFrom,
            NotActiveDates
        }
           
    }
}
