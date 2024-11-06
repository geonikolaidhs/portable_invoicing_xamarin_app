using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eStoreControllerCommand
    {
        [WrmDisplay(Name = "NONE_COMMAND", ResourceType = typeof(ResourcesRest))]//ctrl + .
        NONE = 0,
        [WrmDisplay(Name = "ON_LINE", ResourceType = typeof(ResourcesRest))]
        ON_LINE = 1,
        [WrmDisplay(Name = "OFF_LINE", ResourceType = typeof(ResourcesRest))]
        OFF_LINE = 2,
        [WrmDisplay(Name = "RESTART", ResourceType = typeof(ResourcesRest))]
        RESTART = 4,
    }
}
