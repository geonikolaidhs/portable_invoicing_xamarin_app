using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum VariableSource
    {
        [WrmDisplay(Name = "Field", ResourceType = typeof(ResourcesRest))]
        FIELD,
        [WrmDisplay(Name = "Function", ResourceType = typeof(ResourcesRest))]
        FORMULA
    }
}
