using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eForcedWithdrawMode
    {
        [WrmDisplay(Name = "No", ResourceType = typeof(ResourcesRest))]
        NO,
        [WrmDisplay(Name = "Skippable", ResourceType = typeof(ResourcesRest))]
        SKIPPABLE,
        [WrmDisplay(Name = "Yes", ResourceType = typeof(ResourcesRest))]
        YES
    }
}
