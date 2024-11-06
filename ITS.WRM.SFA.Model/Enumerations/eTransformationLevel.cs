using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eTransformationLevel
    {
        [WrmDisplay(Name = "DEFAULT_TRANSFORM_LEVEL", ResourceType = typeof(ResourcesRest))]
        DEFAULT,//Document is not derived from transformation so the Document Functionality is the default
        [WrmDisplay(Name = "FREEZE_VALUES", ResourceType = typeof(ResourcesRest))]
        FREEZE_VALUES,// Customer, PriceCatalog etc can be modified BUT not The values!
        [WrmDisplay(Name = "FREEZE_EDIT", ResourceType = typeof(ResourcesRest))]
        FREEZE_EDIT,// Nor the values nor Customer,PriceCAtalog etc. cn be modified
        //FULL_EDIT// All Properties may be modified
    }
}
