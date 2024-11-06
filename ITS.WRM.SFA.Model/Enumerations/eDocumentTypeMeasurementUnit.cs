using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eDocumentTypeMeasurementUnit
    {
        [WrmDisplay(Name = "NonOrderUnit", ResourceType = typeof(ResourcesRest))]
        DEFAULT,
        [WrmDisplay(Name = "OrderUnit", ResourceType = typeof(ResourcesRest))]
        PACKING
    }
}
