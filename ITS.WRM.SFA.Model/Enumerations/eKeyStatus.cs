using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eKeyStatus
    {
        [WrmDisplay(Name = "UNKNOWN_POSITION", ResourceType = typeof(ResourcesRest))]
        UNKNOWN = -1,
        [WrmDisplay(Name = "POSITION0", ResourceType = typeof(ResourcesRest))]
        POSITION0 = 0,
        [WrmDisplay(Name = "POSITION1", ResourceType = typeof(ResourcesRest))]
        POSITION1 = 1,
        [WrmDisplay(Name = "POSITION2", ResourceType = typeof(ResourcesRest))]
        POSITION2 = 2,
        [WrmDisplay(Name = "POSITION3", ResourceType = typeof(ResourcesRest))]
        POSITION3 = 3,
        [WrmDisplay(Name = "POSITION4", ResourceType = typeof(ResourcesRest))]
        POSITION4 = 4
    }
}
