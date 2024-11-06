using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eFormatType
    {
        X,
        Z,
        [WrmDisplay(Name = "FormatType_Receipt", ResourceType = typeof(ResourcesRest))]
        Receipt,
        DocumentHeader
    }
}
