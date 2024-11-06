
using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eDiscountSource
    {
        [WrmDisplay(Name = "CUSTOM_DISCOUNT", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        CUSTOM,
        [WrmDisplay(Name = "PRICE_CATALOG_DISCOUNT", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        PRICE_CATALOG,
        [WrmDisplay(Name = "DOCUMENT_DISCOUNT", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        DOCUMENT,
        [WrmDisplay(Name = "CUSTOMER_DISCOUNT", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        CUSTOMER,
        [WrmDisplay(Name = "POINTS_DISCOUNT", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        POINTS,
        [WrmDisplay(Name = "PROMOTION_DISCOUNT", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        PROMOTION_LINE_DISCOUNT,
        [WrmDisplay(Name = "PROMOTION_DISCOUNT", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        PROMOTION_DOCUMENT_DISCOUNT,
        [WrmDisplay(Name = "DEFAULT_DOCUMENT_DISCOUNT", ResourceType = typeof(ITS.WRM.SFA.Resources.ResourcesRest))]
        DEFAULT_DOCUMENT_DISCOUNT

    }
}
