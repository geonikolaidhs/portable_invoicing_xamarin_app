using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using ITS.WRM.SFA.Model.Attributes;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum LicenseServerInstance
    {
        [WrmDisplay(Name = "Undefined", ResourceType = typeof(ResourcesRest))]
        UNDEFINED,
        [WrmDisplay(Name = "MasterOrDual", ResourceType = typeof(ResourcesRest))]
        MASTER_OR_DUAL,
        [WrmDisplay(Name = "STORECONTROLLER", ResourceType = typeof(ResourcesRest))]
        STORE_CONTROLLER
    }
}
