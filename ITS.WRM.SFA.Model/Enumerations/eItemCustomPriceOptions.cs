using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eItemCustomPriceOptions
    {
        [WrmDisplay(Name = "No", ResourceType = typeof(ResourcesRest))]
        NONE,
        [WrmDisplay(Name = "IsOptional", ResourceType = typeof(ResourcesRest))]
        CUSTOM_PRICE_IS_OPTIONAL,
        [WrmDisplay(Name = "IsMandatory", ResourceType = typeof(ResourcesRest))]
        CUSTOM_PRICE_IS_MANDATORY////
    }
}
