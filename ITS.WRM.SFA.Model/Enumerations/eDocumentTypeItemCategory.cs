using ITS.WRM.SFA.Resources;
using System;
using System.Collections.Generic;
using ITS.WRM.SFA.Model.Attributes;
using System.Linq;
using System.Text;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum eDocumentTypeItemCategory
    {

        [WrmDisplay(Name = "NoneSelect", ResourceType = typeof(ResourcesRest))]
        NONE,
        [WrmDisplay(Name = "IncludeItemCategories", ResourceType = typeof(ResourcesRest))]
        INCLUDE_ITEM_CATEGORIES,
        [WrmDisplay(Name = "ExcludeItemCategories", ResourceType = typeof(ResourcesRest))]
        EXCLUDE_ITEM_CATEGORIES,
    }
}
