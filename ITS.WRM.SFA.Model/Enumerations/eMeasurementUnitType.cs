using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eMeasurementUnitType
    {
        [WrmDisplay(Name = "OrderMeasurementUnit", ResourceType = typeof(ResourcesRest))]
        ORDER,
        [WrmDisplay(Name = "PackingMeasurementUnit", ResourceType = typeof(ResourcesRest))]
        PACKING,
        [WrmDisplay(Name = "OrderAndPackingMeasurementUnit", ResourceType = typeof(ResourcesRest))]
        PACKING_AND_ORDER
    }
}
