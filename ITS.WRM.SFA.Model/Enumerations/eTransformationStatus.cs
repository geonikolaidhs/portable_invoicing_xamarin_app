using System;
using System.Collections.Generic;
using System.ComponentModel;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eTransformationStatus
    {
        [WrmDisplay(Name = "CANNOT_BE_TRANSFORMED", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        CANNOT_BE_TRANSFORMED,
        [WrmDisplay(Name = "NOT_TRANSFORMED", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        NOT_TRANSFORMED,
        [WrmDisplay(Name = "PARTIALLY_TRANSFORMED", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        PARTIALLY_TRANSFORMED,
        [WrmDisplay(Name = "FULLY_TRANSFORMED", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        FULLY_TRANSFORMED
    }
}
