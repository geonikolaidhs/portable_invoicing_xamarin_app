using ITS.WRM.SFA.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eItemExecutionMode
    {
        [WrmDisplay(Name = "Discount", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        DISCOUNT,
        [WrmDisplay(Name = "Price", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        SET_PRICE
    }
}