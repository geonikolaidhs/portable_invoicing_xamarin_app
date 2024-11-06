using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum ePaymentMethodType
    {
        [WrmDisplay(Name = "Undefined", ResourceType = typeof(ResourcesRest))]
        UNDEFINED=0,
        [WrmDisplay(Name = "Cash", ResourceType = typeof(ResourcesRest))]
        CASH = 1,
        [WrmDisplay(Name = "Cards", ResourceType = typeof(ResourcesRest))]
        CARDS = 2,
        [WrmDisplay(Name = "Credit", ResourceType = typeof(ResourcesRest))]
        CREDIT = 3,      
    }
}
