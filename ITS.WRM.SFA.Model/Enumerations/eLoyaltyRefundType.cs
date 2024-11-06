using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eLoyaltyRefundType
    {
        [WrmDisplay(Name = "Discount", ResourceType = typeof(ResourcesRest))]
        DISCOUNT,
        [WrmDisplay(Name = "Payment", ResourceType = typeof(ResourcesRest))]
        PAYMENT
    }
}
