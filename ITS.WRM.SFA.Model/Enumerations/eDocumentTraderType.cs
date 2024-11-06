using ITS.WRM.SFA.Resources;
using System;
using ITS.WRM.SFA.Model.Attributes;

namespace ITS.WRM.SFA.Model.Enumerations
{
    [Flags]
    public enum eDocumentTraderType
    {
        [WrmDisplay(Name = "NONE", ResourceType = typeof(ResourcesRest))]
        NONE = 0,
        [WrmDisplay(Name = "Customer", ResourceType = typeof(ResourcesRest))]
        CUSTOMER = 1,
        [WrmDisplay(Name = "Supplier", ResourceType = typeof(ResourcesRest))]
        SUPPLIER = 2,
        [WrmDisplay(Name = "Store", ResourceType = typeof(ResourcesRest))]
        STORE = 4
    }
}
