using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eCurrencyPattern
    {
        [WrmDisplay(Name = "BEFORE_NUMBER", ResourceType = typeof(ResourcesRest))]
        BEFORE_NUMBER= 0,
        [WrmDisplay(Name = "AFTER_NUMBER", ResourceType = typeof(ResourcesRest))]
        AFTER_NUMBER= 1,
        [WrmDisplay(Name = "BEFORE_NUMBER_WITH_SPACE", ResourceType = typeof(ResourcesRest))]
        BEFORE_NUMBER_WITH_SPACE= 2,
        [WrmDisplay(Name = "AFTER_NUMBER_WITH_SPACE", ResourceType = typeof(ResourcesRest))]
        AFTER_NUMBER_WITH_SPACE= 3
    }
}
