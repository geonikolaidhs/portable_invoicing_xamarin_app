using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{

    public enum eModule
    {
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "ALL")]
        ALL,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "NONE")]
        NONE,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "HEADQUARTERS")]
        HEADQUARTERS,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "STORECONTROLLER")]
        STORECONTROLLER,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "DUAL")]
        DUAL,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "POS")]
        POS,
        Mobile,
        SFA
    }
}
