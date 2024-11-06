using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eSex
    {
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "Undefined")]
        UNDEFINED,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "Male")]
        MALE,
        [WrmDisplay(ResourceType = typeof(ResourcesRest), Name = "Female")]
        FEMALE,
        
    }
}
