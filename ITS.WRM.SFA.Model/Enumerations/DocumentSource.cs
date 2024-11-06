using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum DocumentSource
    {
        [WrmDisplay(Name = "WRM_DOCUMENTSOURCE", ResourceType = typeof(ResourcesRest))]
        WRM,
        [WrmDisplay(Name = "POS_DOCUMENTSOURCE", ResourceType = typeof(ResourcesRest))]
        POS,
        [WrmDisplay(Name = "ANDROID_DOCUMENTSOURCE", ResourceType = typeof(ResourcesRest))]
        SFA,
        [WrmDisplay(Name = "MOBILE_DOCUMENTSOURCE", ResourceType = typeof(ResourcesRest))]
        MOBILE,
        [WrmDisplay(Name = "B2C_DOCUMENTSOURCE", ResourceType = typeof(ResourcesRest))]
        B2C,
        [WrmDisplay(Name = "CASHIER_DOCUMENTSOURCE", ResourceType = typeof(ResourcesRest))]
        CASHIER,
    }
}
