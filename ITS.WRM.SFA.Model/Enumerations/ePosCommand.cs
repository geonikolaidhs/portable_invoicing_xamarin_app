using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;

using System.Linq;


namespace ITS.WRM.SFA.Model.Enumerations
{
    [Flags]
    public enum ePosCommand
    {
        [WrmDisplay(Name = "NONE_COMMAND", ResourceType = typeof(ResourcesRest))]
        NONE=0,
        [WrmDisplay(Name = "SEND_CHANGES", ResourceType = typeof(ResourcesRest))]
        SEND_CHANGES = 1,
        [WrmDisplay(Name = "RESTART_POS", ResourceType = typeof(ResourcesRest))]
        RESTART_POS = 2,
        [WrmDisplay(Name = "ISSUE_X", ResourceType = typeof(ResourcesRest))]
        ISSUE_X = 4,
        [WrmDisplay(Name = "ISSUE_Z", ResourceType = typeof(ResourcesRest))]
        ISSUE_Z = 8,
        [WrmDisplay(Name = "RELOAD_ENTITIES", ResourceType = typeof(ResourcesRest))]
        RELOAD_ENTITIES = 16,
        RETRY_IMMEDIATE = 32,
        EXECUTE_POS_SQL = 35,
        EXECUTE_POS_CMD = 67,
        POS_UPDATE = 69,
        POS_APPLICATION_RESTART = 70
    }
}