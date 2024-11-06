using ITS.WRM.SFA.Model.Attributes;
using ITS.WRM.SFA.Resources;

namespace ITS.WRM.SFA.Model.Enumerations
{
    public enum ePriceSuggestionType
    {
        [WrmDisplay(Name = "NoneSelect", ResourceType = typeof(ResourcesRest))]
        NONE,
        [WrmDisplay(Name = "LastPriceType", ResourceType = typeof(ResourcesRest))]
        LAST_PRICE,
        [WrmDisplay(Name = "LastSupplierPriceType", ResourceType = typeof(ResourcesRest))]
        LAST_SUPPLIER_PRICE,
    }
}
