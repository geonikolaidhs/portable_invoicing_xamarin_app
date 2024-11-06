using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum PayPalMode
    {
        [WrmDisplay(Name = "TEST", ResourceType = typeof(ResourcesRest))]
        TEST,
        [WrmDisplay(Name = "PRODUCTION", ResourceType = typeof(ResourcesRest))]
        PRODUCTION
    }
}
