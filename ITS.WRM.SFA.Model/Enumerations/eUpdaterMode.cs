using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eUpdaterMode
    {
        [WrmDisplay(Name = "AUTOMATIC", ResourceType = typeof(ResourcesRest))]
        AUTOMATIC,
        [WrmDisplay(Name = "MANUAL", ResourceType = typeof(ResourcesRest))]
        MANUAL
    }
}
