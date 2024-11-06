using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum PaddingDirection
    {
        [WrmDisplay(Name = "FromLeft", ResourceType = typeof(ResourcesRest))]
        LEFT,
        [WrmDisplay(Name = "FromRight", ResourceType = typeof(ResourcesRest))]
        RIGHT
    }
}
