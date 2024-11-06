using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eDocTypeCustomerCategory
    {

        [WrmDisplay(Name = "NoneSelect", ResourceType = typeof(ResourcesRest))]
        NONE,
        [WrmDisplay(Name = "IncludeCustCategories", ResourceType = typeof(ResourcesRest))]
        INCLUDE_CUSTOMER_CATEGORIES,
        [WrmDisplay(Name = "ExcludeCustCategories", ResourceType = typeof(ResourcesRest))]
        EXCLUDE_CUSTOMER_CATEGORIES,
    }
}
