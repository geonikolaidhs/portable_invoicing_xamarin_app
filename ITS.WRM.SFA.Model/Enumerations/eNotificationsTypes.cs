using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eNotificationsTypes
    {
        [WrmDisplay(Name = "Key", ResourceType = typeof(ResourcesRest))]
        KEY=0,
        [WrmDisplay(Name = "Action", ResourceType = typeof(ResourcesRest))]
        ACTION=1
    }
}
