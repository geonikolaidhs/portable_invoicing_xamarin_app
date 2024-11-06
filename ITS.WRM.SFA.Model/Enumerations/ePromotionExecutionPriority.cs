using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum ePromotionExecutionPriority
    {
        [WrmDisplay(Name = "BestValueForCompany", ResourceType = typeof(ResourcesRest))]
        BEST_VALUE_FOR_CUSTOMER,
        [WrmDisplay(Name = "WorstValueForCompany", ResourceType = typeof(ResourcesRest))]
        WORST_VALUE_FOR_CUSTOMER
    }
}
