using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    /// <summary>
    /// Describes the type of the Coupon amount i.e. it refers to a currency value or to a percentage of the relevant value
    /// </summary>
    public enum CouponAmountType
    {
        [WrmDisplay(Name = "Percentage", ResourceType = typeof(ResourcesRest))]
        PERCENTAGE,
        [WrmDisplay(Name = "Value", ResourceType = typeof(ResourcesRest))]
        VALUE
    }
}
