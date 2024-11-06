using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum GeneratedCouponStatus
    {
        [WrmDisplay(Name = "Requested", ResourceType = typeof(ResourcesRest))]
        Requested,
        [WrmDisplay(Name = "Used", ResourceType = typeof(ResourcesRest))]
        Used
    }
}
